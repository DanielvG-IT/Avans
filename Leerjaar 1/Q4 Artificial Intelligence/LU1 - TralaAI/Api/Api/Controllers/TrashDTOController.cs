using Microsoft.AspNetCore.Mvc;
using Api.Interfaces;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TrashDTOController(ITrashImportService trashImportService) : ControllerBase
    {
        private readonly ITrashImportService _trashImportService = trashImportService;

        [HttpPost("import-trash-data")]
        public async Task<IActionResult> StartImport(CancellationToken cancellationToken = default)
        {
            var result = await _trashImportService.ImportAsync(cancellationToken);
            if (!result)
                return BadRequest("Importing Data failed!");

            return NoContent();
        }
    }
}