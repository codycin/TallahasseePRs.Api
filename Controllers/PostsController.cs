using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TallahasseePRs.Api.DTOs.Posts;
using TallahasseePRs.Api.Services;


namespace TallahasseePRs.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PostsController : ControllerBase
{
    private readonly IPostService _posts;

    public PostsController(IPostService posts)
    {
        _posts = posts;
    }

    //Gets post by ID
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var post = await _posts.GetByIdAsync(id);
        if (post is null) return NotFound();
        return Ok(post);
    }

    //Creates post
    [Authorize]
    [HttpPost]
    
    public async Task<IActionResult> Create([FromBody] CreatePostRequest request) //Creat from JSON body, with a request
    {
        //Get user id securly
        var userId = GetUserId();
        //Create post through post service
        var created = await _posts.CreateAsync(userId, request);

        // 201 with location header is standard REST
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    //Update post
    [Authorize]
    [HttpPut("{id:guid}")]

    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePostRequest request) //From JSON body conver to request
    {
        //Securly get ID
        var userId = GetUserId();

        //Try catch to see if it works so we know what HTTP code to send back
        try
        {
            var updated = await _posts.UpdateAsync(userId, id, request);
            if (updated is null) return NotFound();
            return Ok(updated);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    //Delete ID
    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();

        //Try catch again to check for user
        try
        {
            var deleted = await _posts.DeleteAsync(userId, id);
            if (!deleted) return NotFound();
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
    //Securly get user!
    private Guid GetUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(raw))
            throw new InvalidOperationException("Missing NameIdentifier claim.");
        return Guid.Parse(raw);
    }
}