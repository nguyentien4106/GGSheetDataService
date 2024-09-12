using System.ComponentModel.DataAnnotations.Schema;

namespace DataService.Infrastructure.Entities;

  public abstract class BaseEntity
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
  }
