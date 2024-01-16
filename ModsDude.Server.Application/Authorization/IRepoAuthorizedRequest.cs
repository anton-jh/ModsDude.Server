using MediatR;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using System.Security.Claims;

namespace ModsDude.Server.Application.Authorization;

public interface IRepoAuthorizedRequest : IRequest
{
    ClaimsPrincipal ClaimsPrincipal { get; }
    RepoMembershipLevel MinimumMembershipLevel { get; }


    Task<RepoId> GetRepoId();
}

public interface IRepoAuthorizedRequest<T> : IRequest<T>, IRepoAuthorizedRequest;
