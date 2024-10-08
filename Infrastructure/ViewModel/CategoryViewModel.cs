using Domain.Entity;

namespace Infrastructure.ViewModel;

public class CategoryViewModel
{
    public List<Category> Categories { get; set; }

    public List<LogCategory> LogCategories { get; set; }

    public Category NewCategory { get; set; }
}