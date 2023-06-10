using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace StudySmortAPI.Controllers;

public class FlashcardController : ControllerBase
{

    private readonly DataContext _dbContext;

    public FlashcardController(DataContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
    }
}