using Ballastagram.Comment.Infrastructure.Comment;
using Ballastagram.Post.API.Requests;
using Ballastagram.Post.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ballastagram.Post.API.Controllers
{
    [Produces("application/json")]
    [Route("api/comment")]
    [Authorize]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class CommentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CommentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("get")]
        public async Task<ActionResult<IList<CommentModel>>> GetCommentByIds([FromBody] IList<ulong> ids)
        {
            if (ids is null || !ids.Any())
                return BadRequest();

            List<CommentModel> result = new List<CommentModel>();

            var tasks = new List<Task<IList<CommentModel>>>();

            foreach (var id in ids)
            {
                tasks.Add(_mediator.Send(new CommentMediator.Query(new CommentPK { Id = id })));
            }

            await Task.WhenAll(tasks);

            result = tasks.Select(t => t.Result).SelectMany(c => c).ToList();

            if (result == null || result.Count == 0)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        [Route("add")]
        public async Task<ActionResult<IList<CommentModel>>> CreateComment([FromBody] IList<CommentRequest> inputs)
        {
            if (inputs is null || !inputs.Any())
                return BadRequest();

            List<CommentModel> result = new List<CommentModel>();

            var tasks = new List<Task<CommentModel>>();

            foreach (CommentRequest input in inputs)
            {
                tasks.Add(_mediator.Send(new CommentMediator.AddCommand(CommentRequestConverter.ToModel(input))));
            }

            await Task.WhenAll(tasks);

            result = tasks.Select(t => t.Result).ToList();

            if (result == null || result.Count == 0)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        [Route("edit")]
        public async Task<ActionResult<IList<CommentModel>>> EditComment([FromBody] IList<CommentRequest> inputs)
        {
            if (inputs is null || !inputs.Any())
                return BadRequest();

            List<CommentModel> result = new List<CommentModel>();

            var tasks = new List<Task<CommentModel>>();

            foreach (CommentRequest input in inputs)
            {
                tasks.Add(_mediator.Send(new CommentMediator.EditCommand(CommentRequestConverter.ToModel(input))));
            }

            await Task.WhenAll(tasks);

            result = tasks.Select(t => t.Result).ToList();

            if (result == null || result.Count == 0)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<ActionResult> DeleteComment([FromQuery] ulong commentId)
        {
            if (commentId <= 0)
                return BadRequest("Comment Id must be above 0");

            List<CommentModel> result = new List<CommentModel>();

            await _mediator.Send(new CommentMediator.DeleteCommand(commentId));

            return Ok();
        }
    }
}

