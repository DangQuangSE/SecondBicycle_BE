namespace Domain.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        public IUserRepository Users { get; }
        public IRefreshTokenRepository RefreshTokens { get; }
        public IUserRoleRepository UserRoles { get; }
        Task BeginTransactionAsync();
        Task<int> CommitAsync();
    }
}
