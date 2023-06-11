using System.Xml;
using MarkForth.Storage;

namespace MarkForth.Components;

internal sealed class ComponentService : IComponentService
{

    #region Dependency Injection.

    private readonly IStore<Component> components;

    public ComponentService(
        IStore<Component> components
    )
    {
        this.components = components;
    }

    #endregion DependencyInjection.

    #region IComponentService.

    #region Get.

    public IReadOnlyCollection<Component> Get(Stream input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return this.enumerate(input).ToList().AsReadOnly();
    }

    private IEnumerable<Component> enumerate(Stream input)
    {
        HashSet<string> triedToFindNames = new HashSet<string>();

        using XmlReader reader = XmlReader.Create(input);
        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element && !triedToFindNames.Contains(reader.Name))
            {
                triedToFindNames.Add(reader.Name);

                Component? component = this.components
                    .FirstOrDefault(component => component.Name == reader.Name);

                if (component != null)
                    yield return component;
            }
        }
    }

    #endregion Get.

    public Task<Component> InsertAsync(Component component)
    {
        ArgumentNullException.ThrowIfNull(component);

        return this.components.InsertAsync(component);
    }

    #endregion IComponentService.

}
