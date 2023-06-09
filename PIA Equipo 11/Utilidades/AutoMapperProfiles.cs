﻿using AutoMapper;
using PIA_Equipo_11.DTO;
using PIA_Equipo_11.Entidades;

namespace PIA_Equipo_11.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<UsuarioDTO, Usuario>();
            CreateMap<Usuario, UsuarioDTO>();
            CreateMap<Evento, EventoDTO>();
            CreateMap<EventoDTO, Evento>();
            CreateMap<CredencialesLoginDTO, CredencialesUsuario>();
            CreateMap<Evento, InformacionEventoDTO>();
            CreateMap<PutUsuarioDTO, Usuario>();
            CreateMap<Evento, InformacionExtraEventoDTO>();
            CreateMap<ComentariosUsuario, ComentariosDTO>();
            CreateMap<RegistroEventos, RegistroEventoDTO>();
        }

    }
}
