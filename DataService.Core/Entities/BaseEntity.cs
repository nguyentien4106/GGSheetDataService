using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArchitecture.Core.Entities;

  public abstract class BaseEntity
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
  }
