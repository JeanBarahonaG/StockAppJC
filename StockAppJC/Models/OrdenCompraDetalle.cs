using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StockAppJC.Models
{
    public class OrdenCompraDetalle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ProductoId { get; set; }  // Relación con Producto
        public Producto Producto { get; set; }

        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        public int OrdenCompraId { get; set; }  // Relación con OrdenCompra
        public OrdenCompra OrdenCompra { get; set; }
    }
}
