using LinkUpAPI.Data;
using LinkUpAPI.Models;
using LinkUpAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.JsonWebTokens;
using LinkUpAPI.Services;
using System.IO;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PostsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IBlobStorageService _blobStorageService;

    public PostsController(ApplicationDbContext context, IBlobStorageService blobStorageService)
    {
        _context = context;
        _blobStorageService = blobStorageService;
    }

    [HttpPost]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> CreatePost([FromForm] PostCreateModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        int userId = int.Parse(userIdClaim);

        string mediaUrl = null;
        if (model.MediaFile != null)
        {
            // Générer un nom de fichier unique
            var fileName = $"{Guid.NewGuid()}_{model.MediaFile.FileName}";

            // Télécharger le fichier sur Azure Blob Storage
            mediaUrl = await _blobStorageService.UploadFileAsync(model.MediaFile, fileName);
        }

        // Créer le post
        var post = new Post
        {
            UserId = userId,
            Content = model.Content,
            MediaUrl = mediaUrl
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, post);
    }

    [HttpGet]
    public async Task<IActionResult> GetPosts(int pageNumber = 1, int pageSize = 10)
    {
        var posts = await _context.Posts
            .Include(p => p.User)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PostViewModel
            {
                Id = p.Id,
                Content = p.Content,
                MediaUrl = p.MediaUrl != null ? _blobStorageService.GetBlobSasUri(Path.GetFileName(p.MediaUrl),60) : null,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                User = new UserViewModel
                {
                    Id = p.User.Id,
                    Username = p.User.Username,
                    IsProfilePublic = p.User.IsProfilePublic
                }
            })
            .ToListAsync();

        return Ok(posts);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostById(int id)
    {
        var post = await _context.Posts
            .Include(p => p.User)
            .Where(p => p.Id == id)
            .Select(p => new PostViewModel
            {
                Id = p.Id,
                Content = p.Content,
                MediaUrl = p.MediaUrl != null ? _blobStorageService.GetBlobSasUri(Path.GetFileName(p.MediaUrl), 60) : null,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                User = new UserViewModel
                {
                    Id = p.User.Id,
                    Username = p.User.Username,
                    IsProfilePublic = p.User.IsProfilePublic
                }
            })
            .FirstOrDefaultAsync();

        if (post == null)
        {
            return NotFound();
        }

        return Ok(post);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePost(int id, [FromBody] PostUpdateModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        int userId = int.Parse(userIdClaim);

        var post = await _context.Posts.FindAsync(id);
        if (post == null)
        {
            return NotFound();
        }

        // Vérifier que l'utilisateur est le propriétaire du post
        if (post.UserId != userId)
        {
            return Forbid("Vous n'êtes pas autorisé à modifier ce post.");
        }

        // Mettre à jour le post
        post.Content = model.Content;
        post.MediaUrl = model.MediaUrl;
        post.UpdatedAt = DateTime.UtcNow;

        _context.Posts.Update(post);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        int userId = int.Parse(userIdClaim);

        var post = await _context.Posts.FindAsync(id);
        if (post == null)
        {
            return NotFound();
        }

        // Vérifier que l'utilisateur est le propriétaire du post
        if (post.UserId != userId)
        {
            return Forbid("Vous n'êtes pas autorisé à supprimer ce post.");
        }

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}