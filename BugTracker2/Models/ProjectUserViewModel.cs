﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker2.Models
{
    public class ProjectUserViewModel
        {
            public Project Project { get; set; }
            public MultiSelectList Users { get; set; }
            public string[] SelectedUsers { get; set; }
        }
}