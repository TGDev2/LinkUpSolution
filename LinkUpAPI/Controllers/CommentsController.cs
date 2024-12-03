using LinkUpAPI.Data;
using LinkUpAPI.Models;
using LinkUpAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

[ApiController]
[Route("api/posts/{postId}/comments")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CommentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateComment(int postId, [FromBody] CommentCreateModel model)
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

        // Vérifier si le post existe
        var postExists = await _context.Posts.AnyAsync(p => p.Id == postId);
        if (!postExists)
        {
            return NotFound("La publication n'existe pas.");
        }

        // Créer le commentaire
        var comment = new Comment
        {
            PostId = postId,
            UserId = userId,
            Content = model.Content
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCommentById), new { postId = postId, commentId = comment.Id }, comment);
    }

    [HttpGet]
    public async Task<IActionResult> GetComments(int postId)
    {
        var comments = await _context.Comments
            .Include(c => c.User)
            .Where(c => c.PostId == postId)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new CommentViewModel
            {
                Id = c.Id,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                User = new UserViewModel
                {
                    Id = c.User.Id,
                    Username = c.User.Username,
                    IsProfilePublic = c.User.IsProfilePublic
                }
            })
            .ToListAsync();

        return Ok(comments);
    }

    [HttpGet("{commentId}")]
    public async Task<IActionResult> GetCommentById(int postId, int commentId)
    {
        var comment = await _context.Comments
            .Include(c => c.User)
            .Where(c => c.PostId == postId && c.Id == commentId)
            .Select(c => new CommentViewModel
            {
                Id = c.Id,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                User = new UserViewModel
                {
                    Id = c.User.Id,
                    Username = c.User.Username,
                    IsProfilePublic = c.User.IsProfilePublic
                }
            })
            .FirstOrDefaultAsync();

        if (comment == null)
        {
            return NotFound();
        }

        return Ok(comment);
    }

    [HttpDelete("{commentId}")]
    public async Task<IActionResult> DeleteComment(int postId, int commentId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        int userId = int.Parse(userIdClaim);

        var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.PostId == postId);

        if (comment == null)
        {
            return NotFound();
        }

        // Vérifier que l'utilisateur est le propriétaire du commentaire
        if (comment.UserId != userId)
        {
            return Forbid("Vous n'êtes pas autorisé à supprimer ce commentaire.");
        }

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}