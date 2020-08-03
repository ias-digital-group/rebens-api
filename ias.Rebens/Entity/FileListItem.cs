using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens.Entity
{
    public class FileListItem
    {
        public int Id { get; set; }
        public int IdItem { get; set; }
        public int ItemType { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public DateTime Created { get; set; }
        public string CreatedUserName { get; set; }
    }
}
