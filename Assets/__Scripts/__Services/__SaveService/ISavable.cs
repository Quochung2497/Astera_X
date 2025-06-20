using System.Threading.Tasks;

namespace Course.Services.SaveService
{
    public interface ISavable
    {
        object GetDefault();
        Task Save(ISaveService svc);
        Task Load(ISaveService svc);
    }
}
