using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ballastagram.Post.API.Requests;
using Ballastagram.Post.Infrastructure.Post;
using Ballastagram.Post.Models;
using Ballastagram.Post.Infrastructure.Post;

namespace Ballastagram.API.Controllers
{

    [Produces("application/json")]
    [Route("api/post")]
    [Authorize]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class PostController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PostController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("get")]
        public async Task<ActionResult<IList<PostModel>>> GetPostByIds([FromBody] IList<ulong> ids)
        {
            if (ids is null || !ids.Any())
                return BadRequest();

            IList<PostModel> result = new List<PostModel>();

            var tasks = new List<Task<IList<PostModel>>>();

            foreach (var id in ids)
            {
                tasks.Add(_mediator.Send(new PostMediator.Query(new PostPK { Id = id })));
            }

            await Task.WhenAll(tasks);

            return Ok(tasks.Select(t => t.Result));
        }

        [HttpPost]
        [Route("add")]
        public async Task<ActionResult<IList<PostModel>>> CreatePost([FromBody] IList<PostRequest> inputs)
        {
            if (inputs is null || !inputs.Any())
                return BadRequest();

            IList<PostModel> result = new List<PostModel>();

            var tasks = new List<Task<PostModel>>();

            foreach (PostRequest input in inputs)
            {
                tasks.Add(_mediator.Send(new PostMediator.AddCommand(PostRequestConverter.ToModel(input))));
            }
            
            await Task.WhenAll(tasks);

            return Ok(tasks.Select(t => t.Result));
        }

        [HttpPost]
        [Route("edit")]
        public async Task<ActionResult<IList<PostModel>>> EditPost([FromBody] IList<PostRequest> inputs)
        {
            if (inputs is null || !inputs.Any())
                return BadRequest();

            List<PostModel> result = new List<PostModel>();

            var tasks = new List<Task<PostModel>>();

            foreach (PostRequest input in inputs)
            {
                tasks.Add(_mediator.Send(new PostMediator.EditCommand(PostRequestConverter.ToModel(input))));
            }

            await Task.WhenAll(tasks);

            result = tasks.Select(t => t.Result).ToList();

            if (result == null || result.Count == 0)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<ActionResult<PostModel>> DeletePost([FromQuery] ulong postId)
        {
            if (postId <= 0)
                return BadRequest("Post Id must be above 0");

            List<PostModel> result = new List<PostModel>();

            await _mediator.Send(new PostMediator.DeleteCommand(postId));

            return Ok();
        }
    }
}
