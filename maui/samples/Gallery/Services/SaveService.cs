using System.IO;
using System.Threading.Tasks;

namespace Syncfusion.Maui.ControlsGallery.Services
{
    public partial class SaveService
    {
        //Method to save document as a file and view the saved document.
        public partial void SaveAndView(string filename, string contentType, MemoryStream stream);
    }
}