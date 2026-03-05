using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TallahasseePRs.Api.DTOs.Posts;
using TallahasseePRs.Api.Services;
using TallahasseePRs.Api.Services.PostServices;


namespace TallahasseePRs.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PostsController : ControllerBase
{
    private readonly IPostService _posts;
    private readonly ICurrentUserService _currentUser;
    public PostsController(IPostService posts, ICurrentUserService CurrentUser)
    {
        _posts = posts;
        _currentUser = CurrentUser;
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
        var userId = _currentUser.GetUserId();        //Create post through post service
        var created = await _posts.CreateAsync(userId, request);

        // 201 with location header is standard REST
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    //Update post
    [Authorize]
    [HttpPut("{id:guid}")]

    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePostRequest request) //From JSON body conver to request
    {
        var userId = _currentUser.GetUserId();
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
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var userId = _currentUser.GetUserId();
        var isAdmin = User.IsInRole("Admin"); // uses JWT role claim

        //Try catch again to check for user
        try
        {
            var deleted = await _posts.DeleteAsync(userId, id, isAdmin);
            if (!deleted) return NotFound();
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
    //Securly get user!
}