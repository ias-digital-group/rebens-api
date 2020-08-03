using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    public class FileModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// Nome
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        /// <summary>
        /// Nome do arquivo
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string FileName { get; set; }
        /// <summary>
        /// URL do arquivo
        /// </summary>
        [Required]
        [MaxLength(1000)]
        public string FileURL { get; set; }
        /// <summary>
        /// Id do objeto vinculado a esse arquivo
        /// </summary>
        [Required]
        public int IdItem { get; set; }
        /// <summary>
        /// Tipo do objeto vinculado a esse arquivo
        /// </summary>
        [Required]
        public int ItemType { get; set; }
        public string Created { get; }
        public string CreatedUserName { get; }

        public FileModel() { }

        public FileModel(File file) {
            if (file != null)
            {
                this.Id = file.Id;
                this.Name = file.Name;
                this.FileName = file.FileName;
                this.FileURL = file.FileUrl;
                this.IdItem = file.IdItem;
                this.ItemType = file.ItemType;
                this.Created = TimeZoneInfo.ConvertTimeFromUtc(file.Created, Constant.TimeZone).ToString("dd/MM/yyyy - HH:mm", Constant.FormatProvider);
            }
        }

        public FileModel(Entity.FileListItem file)
        {
            if (file != null)
            {
                this.Id = file.Id;
                this.Name = file.Name;
                this.FileName = file.FileName;
                this.FileURL = file.FileUrl;
                this.IdItem = file.IdItem;
                this.ItemType = file.ItemType;
                this.Created = TimeZoneInfo.ConvertTimeFromUtc(file.Created, Constant.TimeZone).ToString("dd/MM/yyyy - HH:mm", Constant.FormatProvider);
                this.CreatedUserName = file.CreatedUserName;
            }
        }

        public File GetEntity()
        {
            return new File()
            {
                Id = this.Id,
                Name = this.Name,
                FileName = this.FileName,
                FileUrl = this.FileURL,
                IdItem = this.IdItem,
                ItemType = this.ItemType,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
        }
    }
}
