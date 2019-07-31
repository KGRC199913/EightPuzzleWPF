using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace EightPuzzle
{
    class SaveLoadManager : IDao
    {
        public SaveData Load(string location)
        {
            SaveData loadedData = new SaveData();
            BinaryData loadedBinaryData;
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fstream = null;
            try
            {
                fstream = new FileStream(location, FileMode.Open);
                loadedBinaryData = formatter.Deserialize(fstream) as BinaryData;
            } catch (Exception ex)
            {
                return null;
            }
            finally
            {
                fstream?.Close();
            }

            if (loadedBinaryData == null)
            {
                return null;
            }

            loadedData.bitmapImage = DecodePhoto(loadedBinaryData.ByteImage);
            loadedData.location = loadedBinaryData.Location;
            loadedData.time = loadedBinaryData.time;

            return loadedData;
        }

        public void Save(SaveData data, string location)
        {
            BinaryData binaryData = new BinaryData();
            binaryData.ByteImage = EncodePhote(data.bitmapImage);
            binaryData.Location = data.location;
            binaryData.time = data.time;
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fstream = null;
            try
            {
                fstream = new FileStream(location, FileMode.Create);
                formatter.Serialize(fstream, binaryData);
            } catch (Exception ex)
            {
                MessageBox.Show("Save Failed");
            }
            finally
            {
                fstream?.Close();
            }
        }

        public static byte[] EncodePhote(BitmapImage bitmapImage)
        {
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }

            return data;
        }

        public BitmapImage DecodePhoto(byte[] byteVal)
        {
            MemoryStream strmImg = new MemoryStream(byteVal);
            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            myBitmapImage.StreamSource = strmImg;
            myBitmapImage.EndInit();
            return myBitmapImage;
        }
    }
}
