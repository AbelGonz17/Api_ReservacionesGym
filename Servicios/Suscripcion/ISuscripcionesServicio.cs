
using ApiReservacionesGym.DTOs.Suscripcion;
using ApiReservacionesGym.Entidades;
using ApiReservacionesGym.Servicios.Metodos;

namespace ApiReservacionesGym.Servicios.Suscripciones
{
    public interface ISuscripcionesServicio
    {
        Task AsignarSuscripcionInicial(Guid clienteId, int membresiaId);
        Task<ResultadoServicio<List<SuscripcionDetalleDTO>>> ObtenerSuscripciones();
        Task<ResultadoServicio<Suscripcion>> RenovarSuscripcionAsync(Suscripcion suscripcion, decimal montoPago);
    }
}