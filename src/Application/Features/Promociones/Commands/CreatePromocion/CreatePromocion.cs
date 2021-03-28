namespace Application.Features.Promociones.Commands.CreatePromocion
{
    public class CreatePromocionCommandHandler : IRequestHandler<CreatePromocionCommand, Response<int>>
    {
        private readonly IPromocionRepositoryAsync _promocionRepository;
        private readonly IMapper _mapper;
        public CreatePromocionCommandHandler(IPromocionRepositoryAsync promocionRepository, IMapper mapper)
        {
            _promocionRepository = promocionRepository;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(PromocionCommandViewModel request, CancellationToken cancellationToken)
        {
            var promocion = _mapper.Map<Promocion>(request);
            await _promocionRepository.AddAsync(promocion);
            return new Response<int>(promocion.Id);
        }
    }
}