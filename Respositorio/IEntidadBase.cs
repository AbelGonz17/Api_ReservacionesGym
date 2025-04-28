namespace ApiReservacionesGym.Respositorio
{
    public interface IEntidadBase<TId>
    {
        TId Id { get; set; }
    }
}
