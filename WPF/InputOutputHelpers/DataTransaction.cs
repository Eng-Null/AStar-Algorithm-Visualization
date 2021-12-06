using Newtonsoft.Json;
using System.IO;
using System.Windows.Forms;

namespace WPF;

public static class DataTransaction 
{
    public static async Task<Node[,]> LoadAsync()
    {
        OpenFileDialog openFileDialog = new();
        openFileDialog.InitialDirectory = System.Windows.Forms.Application.StartupPath;
        openFileDialog.Title = "Load Json Files";
        openFileDialog.CheckFileExists = true;
        openFileDialog.CheckPathExists = true;
        openFileDialog.DefaultExt = "Json";
        openFileDialog.Filter = "Json files (*.Json)|*.json|All files (*.*)|*.*";
        openFileDialog.FilterIndex = 1;
        openFileDialog.RestoreDirectory = true;
        string openPath = "";

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            openPath = openFileDialog.FileName;
        }
        return await Task.Run(() =>
        {
            if (openPath != "")
            {
                using StreamReader r = new(openPath);
                string json = r.ReadToEnd();
                Node[,] items = JsonConvert.DeserializeObject<Node[,]>(json);
                return items;
            }
            return default;
        });
    }

    public static async Task SaveAsync(Node[,] NodeMap)
    {
        SaveFileDialog saveFileDialog = new();
        saveFileDialog.InitialDirectory = System.Windows.Forms.Application.StartupPath;
        saveFileDialog.Title = "Save Json Files";
        saveFileDialog.CheckPathExists = true;
        saveFileDialog.DefaultExt = "Json";
        saveFileDialog.Filter = "Json files (*.Json)|*.json|All files (*.*)|*.*";
        saveFileDialog.FilterIndex = 1;
        saveFileDialog.RestoreDirectory = true;
        string savePath = "";
        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            savePath = saveFileDialog.FileName;
        }
        await Task.Run(() =>
        {
            if (savePath != "")
            {
                var json = JsonConvert.SerializeObject(NodeMap);
                File.WriteAllText($"{savePath}", json);
            }
        });
    }
}
