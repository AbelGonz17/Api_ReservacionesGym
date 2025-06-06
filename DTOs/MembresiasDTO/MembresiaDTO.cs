﻿using ApiReservacionesGym.Entidades;

namespace ApiReservacionesGym.DTOs.MembresiaDTO
{
    public class MembresiaDTO
    {
        public int Id { get; set; }
        public TipoMembresia Tipo { get; set; }
        public decimal Precio { get; set; }
        public int Duracion { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
        public EstadoMembresia Estado { get; set; }
    }
}
