using ModsDude.Server.Domain.Mods;
using ModsDude.Server.Domain.Profiles;
using ModsDude.Server.Domain.Repos;

namespace ModsDude.Server.Application.Features.Profiles;
public interface IProfileService
{
    Task<CreateProfileResult> Create(RepoId repoId, ProfileName name, CancellationToken cancellationToken);
    Task<DeleteProfileResult> Delete(RepoId repoId, ProfileId id, CancellationToken cancellationToken);
    Task<UpdateProfileResult> Update(RepoId repoId, ProfileId id, ProfileName name, CancellationToken cancellationToken);  
    Task<AddModDependencyResult> AddModDependency(RepoId repoId, ProfileId profileId, ModId modId, ModVersionId modVersionId, bool lockVersion, CancellationToken cancellationToken);
    Task<UpdateModDependencyResult> UpdateModDependency(RepoId repoId, ProfileId profileId, ModId modId, ModVersionId modVersionId, bool lockVersion, CancellationToken cancellationToken);
    Task<DeleteModDependencyResult> DeleteModDependency(RepoId repoId, ProfileId profileId, ModId modId, CancellationToken cancellationToken);
}


public abstract record CreateProfileResult
{
    public record Ok(Profile Profile) : CreateProfileResult;
    public record NameTaken : CreateProfileResult;
}

public abstract record UpdateProfileResult
{
    public record Ok(Profile Profile) : UpdateProfileResult;
    public record NameTaken : UpdateProfileResult;
    public record NotFound : UpdateProfileResult;
}

public abstract record DeleteProfileResult
{
    public record Ok : DeleteProfileResult;
    public record NotFound : DeleteProfileResult;
}

public abstract record AddModDependencyResult
{
    public record Ok(ModDependency ModDependency) : AddModDependencyResult;
    public record ProfileNotFound : AddModDependencyResult;
    public record ModNotFound : AddModDependencyResult;
    public record AlreadyExists : AddModDependencyResult;
}

public abstract record UpdateModDependencyResult
{
    public record Ok(ModDependency ModDependency) : UpdateModDependencyResult;
    public record ProfileNotFound : UpdateModDependencyResult;
    public record DependencyNotFound : UpdateModDependencyResult;
    public record ModVersionNotFound : UpdateModDependencyResult;
}

public abstract record DeleteModDependencyResult
{
    public record Ok : DeleteModDependencyResult;
    public record ProfileNotFound : DeleteModDependencyResult;
    public record DependencyNotFound : DeleteModDependencyResult;
}   