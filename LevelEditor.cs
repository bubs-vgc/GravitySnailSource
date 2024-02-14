using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace GravitySnail
{
    /// <summary>
    /// Deals with interactions between level editor and gravity snail
    /// </summary>
    internal class LevelEditor
    {
        /// <summary>
        /// Imports a level
        /// </summary>
        /// <param name="path">Path to level file</param>
        /// <param name="levelNumber"></param>
        /// <returns></returns>
        public static Level LoadLevel(string path, int levelNumber)
        {
            int mapWidth;
            int mapHeight;

            {
                //makes the tiles
                Level loadedLevel = null;
                StreamReader input = null;
                try
                {
                    input = new StreamReader("../../../" + path);
                    mapWidth = int.Parse(input.ReadLine());
                    mapHeight = int.Parse(input.ReadLine());
                    loadedLevel = new Level(levelNumber, mapWidth, mapHeight);
                    String line = null;

                        for (int i = 0; i < mapWidth; i++)
                        {
                            for (int j = 0; j < mapHeight; j++)
                            {
                                line = input.ReadLine();
                                loadedLevel.levelData[i, j] = line;
                            }
                        }
                }
                catch (Exception e)
                {
                    Debug.Fail("Level load failed (probable file not found): " + e.Message);
                }
                finally
                {
                    if (input != null)
                    {
                        input.Close();
                    }
                    
                }
                return loadedLevel;
            }
        }
    }
}
