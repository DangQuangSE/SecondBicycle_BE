namespace Domain.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        public IAuthRepository Users { get; }
        public IRefreshTokenRepository RefreshTokens { get; }
        public IUserRoleRepository UserRoles { get; }
        Task BeginTransactionAsync();
        Task<int> CommitAsync();
    }
}
