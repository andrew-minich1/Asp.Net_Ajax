using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReadJsonFile.Models
{
    public class CommentViewModel
    {
        public List<Comment> Comments { get; set; }
        public int[] Pages { get; set; }
    }
}