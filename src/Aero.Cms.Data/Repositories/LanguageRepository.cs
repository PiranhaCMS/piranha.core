

using Aero.Cms.Models;
using Aero.Cms.Repositories;
using Marten;


namespace Aero.Cms.Data.Repositories;

internal class LanguageRepository : ILanguageRepository
{
    private readonly IDb _db;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="db">The current db context</param>
    public LanguageRepository(IDb db)
    {
        _db = db;
    }
    

    /// <summary>
    /// Gets all available models.
    /// </summary>
    /// <returns>The available models</returns>
    public async Task<IEnumerable<Language>> GetAll()
    {
        return await _db.Languages
            .OrderBy(l => l.Title)
            .Select(l => new Language
            {
                Id = l.Id,
                Culture = l.Culture,
                IsDefault = l.IsDefault,
                Title = l.Title
            })
            .ToListAsync()
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <returns>The model, or null if it doesn't exist</returns>
    public async Task<Language> GetById(string id)
    {
        return await _db.Languages
            .Where(l => l.Id == id)
            .Select(l => new Language
            {
                Id = l.Id,
                Culture = l.Culture,
                IsDefault = l.IsDefault,
                Title = l.Title
            })
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the default model.
    /// </summary>
    /// <returns>The model</returns>
    public async Task<Language> GetDefault()
    {
        return await _db.Languages
            .Where(l => l.IsDefault)
            .Select(l => new Language
            {
                Id = l.Id,
                Culture = l.Culture,
                IsDefault = l.IsDefault,
                Title = l.Title
            })
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Adds or updates the given model in the database
    /// depending on its state.
    /// </summary>
    /// <param name="model">The model</param>
    public async Task Save(Language model)
    {
        var language = await _db.Languages
            .FirstOrDefaultAsync(l => l.Id == model.Id)
            .ConfigureAwait(false);

        if (language == null)
        {
            language = new Data.Language
            {
                Id = !string.IsNullOrEmpty(model.Id) ? model.Id : Snowflake.NewId().ToString()
            };
            //await _db.Languages.AddAsync(language).ConfigureAwait(false);
            _db.session.Store(language);
        }
        language.Culture = model.Culture;
        language.IsDefault = model.IsDefault;
        language.Title = model.Title;

        await _db.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes the model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    public async Task Delete(string id)
    {
        var language = await _db.Languages
            .FirstOrDefaultAsync(l => l.Id == id)
            .ConfigureAwait(false);

        if (language != null)
        {
            //_db.Languages.Remove(language);
            _db.session.Delete(language);

            await _db.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}

