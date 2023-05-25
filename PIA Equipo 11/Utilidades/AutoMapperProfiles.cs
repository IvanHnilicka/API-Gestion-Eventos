using AutoMapper;
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
            CreateMap<Evento,EventoDTO>();
            CreateMap<EventoDTO, Evento>();
            /*CreateMap<Alumno, AlumnoDTOConClases>()
                .ForMember(alumnoDTO => alumnoDTO.Clases, opciones => opciones.MapFrom(MapAlumnoDTOClases));*/
        }

    }
}
