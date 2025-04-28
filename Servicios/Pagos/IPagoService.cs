using ApiReservacionesGym.DTOs.Pago;
using ApiReservacionesGym.Servicios.Metodos;

namespace ApiReservacionesGym.Servicios.Pagos
{
    public interface IPagoService
    {
        Task<ResultadoServicio<PagoDTO>> ObtenerPago(int id);
        Task<ResultadoServicio<List<PagoDTO>>> ObtenerPagos();
        Task<ResultadoServicio<string>> RealizarPago(CreacionPagoDTO creacionPagoDTO);
    }
}