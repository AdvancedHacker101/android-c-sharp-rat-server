using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AndroidExtension
{
    /// <summary>
    /// Handling the displaying of images
    /// </summary>
    public partial class ImageView : Form
    {
        #region Variables

        /// <summary>
        /// Indicates if we need constant rotation
        /// </summary>
        private bool streamMode;
        /// <summary>
        /// Constant rotation value, in case of stream
        /// </summary>
        private int constRotate = 0;

        #endregion

        #region UI Functions

        /// <summary>
        /// Constructor
        /// </summary>
        public ImageView()
        {
            InitializeComponent(); //Load controls
            streamMode = false; //Not streaming
        }

        /// <summary>
        /// Alternate Constructor
        /// </summary>
        /// <param name="isStreaming">Indicates if we need to constantly rotate images</param>
        public ImageView(bool isStreaming)
        {
            InitializeComponent(); //Load Controls
            streamMode = isStreaming; //Set the streaming value
        }

        /// <summary>
        /// Update the image to display
        /// </summary>
        /// <param name="img">The image to display</param>
        public void UpdateImage(Image img)
        {
            if (pictureBox1.Image != null) pictureBox1.Image.Dispose(); //Dispose the previous image
            if (streamMode && constRotate != 0) img = RotateImage(img, constRotate); //if streaming apply constant rotation
            pictureBox1.Image = img; //Display the image
        }

        #pragma warning disable IDE1006

        /// <summary>
        /// Rotate Left Tool Strip Menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rotateLeft90ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null) //If we have an image
            {
                pictureBox1.Image = RotateImage(pictureBox1.Image, -90); //Rotate the image
                if (streamMode) constRotate -= 90; //If streaming save constant rotate
            }
        }

        /// <summary>
        /// Rotate Right Tool Strip Item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rotateRight90ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null) //If we have an image
            {
                pictureBox1.Image = RotateImage(pictureBox1.Image, 90); //Rotate the image
                if (streamMode) constRotate += 90; //If streaming save the constant rotation
            }
        }

        /// <summary>
        /// Rotate Image
        /// </summary>
        /// <param name="img">The input image</param>
        /// <param name="rotationAngle">The rotation angle</param>
        /// <returns>The rotated image</returns>

        public static Image RotateImage(Image img, float rotationAngle)
        {
            //https://stackoverflow.com/questions/2163829/how-do-i-rotate-a-picture-in-winforms
            Bitmap bmp = new Bitmap(img.Width, img.Height);
            Graphics gfx = Graphics.FromImage(bmp);
            gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);
            gfx.RotateTransform(rotationAngle);
            gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gfx.DrawImage(img, new Point(0, 0));
            gfx.Dispose();
            return bmp;
        }

        #endregion
    }
}
