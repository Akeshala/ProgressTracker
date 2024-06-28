using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ProgressTracker.ViewModels.Shared;
public class DropdownModel
{
    public int SelectedValue { get; set; }
    public List<SelectListItem> Options { get; set; }
}