using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    public class PermissionModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? IdParent { get; set; }
        public List<PermissionModel> Permissions { get; set; }

        public PermissionModel() { }

        public PermissionModel(Permission permission) {
            this.Id = permission.Id;
            this.Name = permission.Name;
            this.IdParent = permission.IdParent;
            this.Permissions = new List<PermissionModel>();

            if (permission.Permissions != null)
            {
                foreach (var per in permission.Permissions)
                    this.Permissions.Add(new PermissionModel(per));
            }
        }
    }
}
