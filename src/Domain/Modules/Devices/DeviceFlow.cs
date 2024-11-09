using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Modules.Devices;

public class DeviceFlow
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [MaxLength(50)]

    public string UserCode { get; set; }

    [Required]
    [MaxLength(50)]

    public string DeviceCode { get; set; }

    [Required]
    public DateTime ExpireIn { get; set; }

    [Required]
    [MaxLength(100)]
    public string ClientId { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; }

    [Required]
    [MaxLength(150)]
    public string SessionId { get; set; }

    public bool? UserInteractionComplete { get; set; }

    public string RequestedScope { get; set; }
}
