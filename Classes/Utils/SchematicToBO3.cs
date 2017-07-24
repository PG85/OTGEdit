using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using fNbt;

namespace OTGE.Utils
{
    public class Coordinates : IEquatable<Coordinates>
    {
        public int X;
        public int Y;
        public int Z;

        public Coordinates(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public bool Equals(Coordinates obj)
        {
            if (obj.X == X && obj.Y == Y && obj.Z == Z)
            {
                return true;
            }
            return false;
        }
    }

    public class BaseBlock
    {
        public NbtCompound NbtData;
        public short Index;
        public byte Data;

        public BaseBlock(short index, byte data)
        {
            Index = index;
            Data = data;
        }
    }

    public class SchematicToBO3
    {
        public static void doSchematicToBO3(System.IO.FileInfo loadFile, System.IO.DirectoryInfo saveDir, bool exportForTC, bool useBranches, int centerBlockId, bool removeAir)
        {
            // Most of the code for reading NBT files was ported from WorldEdit!

            if (loadFile.Name.EndsWith(".schematic"))
            {
                string filePath = loadFile.Name;
                string fileName = filePath.Replace(".schematic","");

                //- Loading a gzipped file:
                var myFile = new NbtFile();
                myFile.LoadFromFile(loadFile.FullName);
                NbtCompound schematicTag = myFile.RootTag;

                // Schematic tag
                if (!schematicTag.Name.Equals("Schematic"))
                {
                    throw new Exception("Tag \"Schematic\" does not exist or is not first");
                }

                // Check
                IEnumerable<NbtTag> schematic = schematicTag.Tags;

                if (!schematic.Any(a => a.Name.Equals("Blocks")))
                {
                    throw new Exception("Schematic file is missing a \"Blocks\" tag");
                }

                // Get information
                short width = schematicTag.Get<NbtShort>("Width").Value;
                short length = schematicTag.Get<NbtShort>("Length").Value;
                short height = schematicTag.Get<NbtShort>("Height").Value;

                // Check type of Schematic
                string materials = schematicTag.Get<NbtString>("Materials").Value;
                if (!materials.Equals("Alpha"))
                {
                    throw new Exception("Schematic file is not an Alpha schematic");
                }

                // Get blocks
                byte[] blockId = schematicTag.Get<NbtByteArray>("Blocks").Value;
                byte[] blockData = schematicTag.Get<NbtByteArray>("Data").Value;
                byte[] addId = new byte[0];
                short[] blocks = new short[blockId.Length]; // Have to later combine IDs

                // We support 4096 block IDs using the same method as vanilla Minecraft, where
                // the highest 4 bits are stored in a separate byte array.
                if (schematic.Any(a => a.Name.Equals("AddBlocks")))
                {
                    addId = schematicTag.Get<NbtByteArray>("AddBlocks").Value;
                }

                // Combine the AddBlocks data with the first 8-bit block ID
                for (int index = 0; index < blockId.Length; index++)
                {
                    if ((index >> 1) >= addId.Length)
                    {
                        // No corresponding AddBlocks index
                        blocks[index] = (short) (blockId[index] & 0xFF);
                    } else {
                        if ((index & 1) == 0)
                        {
                            blocks[index] = (short) (((addId[index >> 1] & 0x0F) << 8) + (blockId[index] & 0xFF));
                        } else {
                            blocks[index] = (short) (((addId[index >> 1] & 0xF0) << 4) + (blockId[index] & 0xFF));
                        }
                    }
                }

                // Need to pull out tile entities
                NbtList tileEntities = schematicTag.Get<NbtList>("TileEntities");
                List<Tuple<Coordinates, String, List<NbtTag>>> tileEntitiesMap = new List<Tuple<Coordinates, String, List<NbtTag>>>();

                foreach (NbtTag tag in tileEntities)
                {
                    if (!(tag is NbtCompound)) continue;
                    NbtCompound t = (NbtCompound) tag;

                    String name = null;
                    NbtTag idTag = null;
                    int x = 0;
                    int y = 0;
                    int z = 0;

                    List<NbtTag> values = new List<NbtTag>();

                    foreach (NbtTag entry in t.Tags)
                    {
                        if (entry.Name.Equals("x"))
                        {
                            if (entry.TagType == NbtTagType.Int)
                            {
                                x = entry.IntValue;
                            }
                        }
                        else if (entry.Name.Equals("y"))
                        {
                            if (entry.TagType == NbtTagType.Int)
                            {
                                y = entry.IntValue;
                            }
                        }
                        else if (entry.Name.Equals("z"))
                        {
                            if (entry.TagType == NbtTagType.Int)
                            {
                                z = entry.IntValue;
                            }
                        }
                        else if (entry.Name.Equals("id"))
                        {
                            if (entry.TagType == NbtTagType.String)
                            {
                                idTag = entry;
                                name = entry.StringValue;
                            }
                        }

                        if(!entry.Name.Equals("x") && !entry.Name.Equals("y") && !entry.Name.Equals("z") && !entry.Name.Equals("id"))
                        {
                            values.Add(entry);
                        }
                    }

                    if(values.Count > 0)
                    {
                        values.Add(idTag);
                        Coordinates vec = new Coordinates(x, y, z);
                        tileEntitiesMap.Add(new Tuple<Coordinates, String, List<NbtTag>>(vec, name, values));
                    }
                }

                int tileEntityCount = 1;

                int widthMin = width;
                int heightMin = height;
                int lengthMin = length;
                int widthMax = int.MinValue;
                int heightMax = int.MinValue;
                int lengthMax = int.MinValue;

                bool centerBlockFound = false;
                int centerPointX = 0;
                int centerPointY = 0;
                int centerPointZ = 0;

                for (int x = 0; x < width; ++x)
                {
                    for (int y = 0; y < height; ++y)
                    {
                        for (int z = 0; z < length; ++z)
                        {
                            int index = y * width * length + z * width + x;
                            short blockMaterial = blocks[index];

                            if (centerBlockId != -1 && blockMaterial == centerBlockId)
                            {
                                centerPointX = x;
                                centerPointY = y;
                                centerPointZ = z;
                                centerBlockFound = true;
                            }

                            if (!removeAir || blockMaterial != 0) // Ignore AIR(?)
                            {
                                if (x < widthMin)
                                {
                                    widthMin = x;
                                }
                                if (y < heightMin)
                                {
                                    heightMin = y;
                                }
                                if (z < lengthMin)
                                {
                                    lengthMin = z;
                                }

                                if (x > widthMax)
                                {
                                    widthMax = x;
                                }
                                if (y > heightMax)
                                {
                                    heightMax = y;
                                }
                                if (z > lengthMax)
                                {
                                    lengthMax = z;
                                }
                            }
                        }
                    }
                }

                int bO3Width = (short)(widthMax - widthMin) + 1;
                int bO3Height = (short)(heightMax - heightMin);
                int bO3Length = (short)(lengthMax - lengthMin) + 1;

                if (centerBlockId == -1 || !centerBlockFound)
                {
                    centerPointX = (int)Math.Floor(bO3Width / 2d);
                    centerPointY = 0;
                    centerPointZ = (int)Math.Floor(bO3Length / 2d);
                }

                StringBuilder[,] blocksPerChunkArr = new StringBuilder[Math.Abs(widthMin - widthMax) + 1, Math.Abs(lengthMin - lengthMax) + 1];

                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(saveDir.FullName + "\\" + fileName);
                if (!dir.Exists)
                {
                    dir.Create();
                    dir.Refresh();
                }

                System.Security.AccessControl.DirectorySecurity sec = dir.GetAccessControl();
                System.Security.AccessControl.FileSystemAccessRule accRule = new System.Security.AccessControl.FileSystemAccessRule(Environment.UserDomainName + "\\" + Environment.UserName, System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.AccessControlType.Allow);
                sec.AddAccessRule(accRule);

                for (int x = widthMin; x <= widthMax; ++x)
                {
                    for (int y = heightMin; y <= heightMax; ++y)
                    {
                        for (int z = lengthMin; z <= lengthMax; ++z)
                        {
                            StringBuilder blocksInChunk = new StringBuilder();
                            Coordinates chunkCoordinates = null;

                            if (useBranches)
                            {
                                chunkCoordinates = new Coordinates((int)Math.Floor(x / 16d), 0, (int)Math.Floor(z / 16d));
                            } else {
                                chunkCoordinates = new Coordinates(0, 0, 0);
                            }

                            if(blocksPerChunkArr[chunkCoordinates.X, chunkCoordinates.Z] == null)
                            {
                                blocksPerChunkArr[chunkCoordinates.X, chunkCoordinates.Z] = blocksInChunk;
                            } else {
                                blocksInChunk = blocksPerChunkArr[chunkCoordinates.X, chunkCoordinates.Z];
                            }

                            int index = y * width * length + z * width + x;
                            byte blockSubType = blockData[index];
                            short blockMaterial = blocks[index];

                            if (blockMaterial == 18)
                            {
                                if (blockSubType == 4 || blockSubType == 8 || blockSubType == 12)
                                {
                                    blockSubType = 0;
                                }
                                if (blockSubType == 5 || blockSubType == 9 || blockSubType == 13)
                                {
                                    blockSubType = 1;
                                }
                                if (blockSubType == 6 || blockSubType == 10 || blockSubType == 14)
                                {
                                    blockSubType = 2;
                                }
                                if (blockSubType == 7 || blockSubType == 11 || blockSubType == 15)
                                {
                                    blockSubType = 3;
                                }
                            }

                            if (blockMaterial == 161)
                            {
                                if (blockSubType == 4 || blockSubType == 8 || blockSubType == 12)
                                {
                                    blockSubType = 0;
                                }
                                if (blockSubType == 5 || blockSubType == 9 || blockSubType == 13)
                                {
                                    blockSubType = 1;
                                }
                            }

                            if (!removeAir || blockMaterial != 0) // Ignore AIR(?)
                            {
                                String tileEntityName = "";
                                Tuple<Coordinates, String, List<NbtTag>> extraData = tileEntitiesMap.FirstOrDefault(a => a.Item1.Equals(new Coordinates(x,y,z)));
                                if (extraData != null)
                                {                                    
                                    tileEntityName = extraData.Item2 + ".nbt";

                                    // Save entityData as seperate file
                                    NbtFile entityFile = new NbtFile();
                                    foreach (NbtTag nbtTag in extraData.Item3)
                                    {
                                        entityFile.RootTag.Add((NbtTag)nbtTag.Clone());
                                    }

                                    string entityFileName = "";

                                    if (useBranches)
                                    {
                                        entityFileName = dir.FullName + "\\" + fileName + "_" + (x - (chunkCoordinates.X * 16) - 8) + "_" + y + "_" + (z - (chunkCoordinates.Z * 16) - 7) + "_" + tileEntityName;
                                    } else {
                                        entityFileName = dir.FullName + "\\" + fileName + "_" + (x - centerPointX) + "_" + (y - centerPointY) + "_" + (z - centerPointZ) + "_" + tileEntityName;
                                    }

                                    entityFile.SaveToFile(entityFileName, NbtCompression.GZip);

                                    tileEntityCount += 1;
                                }

                                if(useBranches)
                                {
                                    blocksInChunk.Append("Block(" + (x - (chunkCoordinates.X * 16) - 8) + "," + y + "," + (z - (chunkCoordinates.Z * 16) - 7) + "," + blockMaterial + (blockSubType != 0 ? ":" + blockSubType : "") + (tileEntityName.Length > 0 ? "," + tileEntityName : "") + ")\r\n");
                                } else {
                                    blocksInChunk.Append("Block(" + (x - centerPointX) + "," + (y - centerPointY) + "," + (z - centerPointZ) + "," + blockMaterial + (blockSubType != 0 ? ":" + blockSubType : "") + (tileEntityName.Length > 0 ? "," + tileEntityName : "") + ")\r\n");
                                }
                            }
                        }
                    }
                }

                StringBuilder BO3String;

                System.Security.AccessControl.DirectorySecurity sec2 = saveDir.GetAccessControl();
                System.Security.AccessControl.FileSystemAccessRule accRule2 = new System.Security.AccessControl.FileSystemAccessRule(Environment.UserDomainName + "\\" + Environment.UserName, System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.AccessControlType.Allow);
                sec2.AddAccessRule(accRule2);

                bool saveSubDirCreated = false;
                bool firstPass = true;

                for (int x1 = 0; x1 <= Math.Abs(widthMin - widthMax); x1++)
                {
                    for (int z1 = 0; z1 <= Math.Abs(lengthMin - lengthMax); z1++)
                    {
                        StringBuilder sb = blocksPerChunkArr[x1, z1];
                        if(sb == null)
                        {
                            continue;
                        }

                        KeyValuePair<Coordinates, StringBuilder> chunk = new KeyValuePair<Coordinates, StringBuilder>(new Coordinates(x1, 0, z1), sb);

                        for (int i = 0; i < 1; i++)
                        {
                            BO3String = new StringBuilder();
                            BO3String.Append("#######################################################################\r\n");
                            BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                            BO3String.Append("# |                            BO3 object                           | #\r\n");
                            BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                            BO3String.Append("#######################################################################\r\n");
                            BO3String.Append("\r\n");
                            if (!useBranches || exportForTC)
                            {
                                BO3String.Append("# The descriptions in this file are accurate only for TerrainControl and OTG.\r\n");
                                BO3String.Append("# For MCW and OTG+ these descriptions are only accurate if the BO3 is used as a CustomObject.\r\n");
                                BO3String.Append("# MCW and OTG+ completely reimplemented CustomStructure, check the MCW/OTG+ documentation for more information.\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# This is the config file of a custom object.\r\n");
                                BO3String.Append("# If you add this object correctly to your BiomeConfigs, it will spawn in the world.\r\n");                    
                                BO3String.Append("\r\n");
                                BO3String.Append("# This is the creator of this BO3 object\r\n");
                                BO3String.Append("Author: Unknown\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# A short description of this BO3 object\r\n");
                                BO3String.Append("Description: This BO3 was converted from a schematic using OTGE. The author and description information of the schematic could not be found. If you know who the author of the schematic is then please add this information.\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# The BO3 version, don't change this! It can be used by external applications to do a version check.\r\n");
                                BO3String.Append("Version: 3\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# The settings mode, WriteAll, WriteWithoutComments or WriteDisable. See WorldConfig.\r\n");
                                BO3String.Append("SettingsMode: WriteDisable\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("#######################################################################\r\n");
                                BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                BO3String.Append("# |                          Main settings                          | #\r\n");
                                BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                BO3String.Append("#######################################################################\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# This needs to be set to true to spawn the object in the Tree and Sapling resources.\r\n");
                                BO3String.Append("Tree: false\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# The frequency of the BO3 from 1 to 200. Tries this many times to spawn this BO3 when using the CustomObject(...) resource.\r\n");
                                BO3String.Append("# Ignored by Tree(..), Sapling(..) and CustomStructure(..)\r\n");
                                BO3String.Append("Frequency: 40\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# The rarity of the BO3 from 0 to 100. Each spawn attempt has rarity% chance to succeed when using the CustomObject(...) resource.\r\n");
                                BO3String.Append("# Ignored by Tree(..), Sapling(..) and CustomStructure(..)\r\n");
                                BO3String.Append("Rarity: 100.0\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# If you set this to true, the BO3 will be placed with a random rotation.\r\n");
                                BO3String.Append("RotateRandomly: false\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# The spawn height of the BO3 - randomY, highestBlock or highestSolidBlock.\r\n");
                                BO3String.Append("SpawnHeight: highestBlock\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# The height limits for the BO3.\r\n");
                                BO3String.Append("MinHeight: 0\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("MaxHeight: 256\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Objects can have other objects attached to it: branches. Branches can also\r\n");
                                BO3String.Append("# have branches attached to it, which can also have branches, etc. This is the\r\n");
                                BO3String.Append("# maximum branch depth for this objects.\r\n");
                                BO3String.Append("MaxBranchDepth: 10\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# When spawned with the UseWorld keyword, this BO3 should NOT spawn in the following biomes.\r\n");
                                BO3String.Append("# If you writer.write the BO3 name directly in the BiomeConfigs, this will be ignored.\r\n");
                                BO3String.Append("ExcludedBiomes: All\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# To optimise this BO3 for faster loading times you can use Notepad++ to find/replace comments and abbreviate things.\r\n");
                                BO3String.Append("# Notepad++ find/replace with regex:\r\n");
                                BO3String.Append("# Find: ^[#;].* replace to nothing removes all lines starting with # \r\n");
                                BO3String.Append("# Find: ^\\s*$ replace to nothing removes most empty lines \r\n");
                                BO3String.Append("# Notepad++ find/replace without regex:\r\n");
                                BO3String.Append("# *Warning: This will only work for OTG 1.10.2 v21 / OTG 1.11.2 v6 or higher. B( is not recognised by lower versions.\r\n");
                                BO3String.Append("# Find: Block( replace to B(\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("#######################################################################\r\n");
                                BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                BO3String.Append("# |                      Source block settings                      | #\r\n");
                                BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                BO3String.Append("#######################################################################\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# The block(s) the BO3 should spawn in.\r\n");
                                BO3String.Append("SourceBlocks: AIR\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# The maximum percentage of the BO3 that can be outside the SourceBlock.\r\n");
                                BO3String.Append("# The BO3 won't be placed on a location with more blocks outside the SourceBlock than this percentage.\r\n");
                                BO3String.Append("MaxPercentageOutsideSourceBlock: 100\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# What to do when a block is about to be placed outside the SourceBlock? (dontPlace, placeAnyway)\r\n");
                                BO3String.Append("OutsideSourceBlock: placeAnyway\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("#######################################################################\r\n");
                                BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                BO3String.Append("# |                              Blocks                             | #\r\n");
                                BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                BO3String.Append("#######################################################################\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# All the blocks used in the BO3 are listed here. Possible blocks:\r\n");
                                BO3String.Append("# Block(x,y,z,id[.data][,nbtfile.nbt) or B(x,y,z,id[.data][,nbtfile.nbt)\r\n");
                                BO3String.Append("# RandomBlock(x,y,z,id[:data][,nbtfile.nbt],chance[,id[:data][,nbtfile.nbt],chance[,...]])\r\n");
                                BO3String.Append("# So RandomBlock(0,0,0,CHEST,chest.nbt,50,CHEST,anotherchest.nbt,100) will spawn a chest at\r\n");
                                BO3String.Append("# the BO3 origin, and give it a 50% chance to have the contents of chest.nbt, or, if that\r\n");
                                BO3String.Append("# fails, a 100% percent chance to have the contents of anotherchest.nbt.\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append(chunk.Value);
                                if(chunk.Value.Length > 0)
                                {
                                    BO3String.Append("\r\n");
                                }
                                BO3String.Append("\r\n");
                                BO3String.Append("#######################################################################\r\n");
                                BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                BO3String.Append("# |                             Entities                            | #\r\n");
                                BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                BO3String.Append("#######################################################################\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Entity() spawns an entity instead of a block. The entity is spawned only once when the BO3 is spawned.\r\n");
                                BO3String.Append("# Entities are persistent by default so they don't de-spawn when no player is near, they are only unloaded.\r\n");
                                BO3String.Append("# Usage: Entity(x,y,z,mobName,groupSize,NameTagOrNBTFileName) or Entity(x,y,z,mobName,groupSize)\r\n");
                                BO3String.Append("# Use /tc entities or /otg entities to get a list of entities that can be used as mobName, this includes entities added by other mods.\r\n");
                                BO3String.Append("# NameTagOrNBTFileName can be either a nametag for the mob or an nbt file (such as mymobinfo.nbt or mymobinfo.txt).\r\n");
                                BO3String.Append("# When using a text file you can use the same mob spawning parameters used with the /summon command to equip the\r\n");
                                BO3String.Append("# entity and give it custom attributes etc. You can copy the DATA part of a summon command including surrounding\r\n");
                                BO3String.Append("# curly braces to a .txt file, for instance for: \"/summon Skeleton x y z {DATA}\"\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("#######################################################################\r\n");
                                BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                BO3String.Append("# |                            BO3 checks                           | #\r\n");
                                BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                BO3String.Append("#######################################################################\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Require a condition at a certain location in order for the BO3 to be spawned.\r\n");
                                BO3String.Append("# BlockCheck(x,y,z,BlockName[,BlockName[,...]]) - one of the blocks must be at the location\r\n");
                                BO3String.Append("# BlockCheckNot(x,y,z,BlockName[,BlockName[,...]]) - all the blocks must not be at the location\r\n");
                                BO3String.Append("# LightCheck(x,y,z,minLightLevel,maxLightLevel) - light must be between min and max (inclusive)\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# You can use \"Solid\" as a BlockName for matching all solid blocks or \"All\" to match all blocks that aren't air.\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Examples:\r\n");
                                BO3String.Append("#   BlockCheck(0,-1,0,GRASS,DIRT)  Require grass or dirt just below the object\r\n");
                                BO3String.Append("#   BlockCheck(0,-1,0,Solid)       Require any solid block just below the object\r\n");
                                BO3String.Append("#   BlockCheck(0,-1,0,WOOL)        Require any type of wool just below the object\r\n");
                                BO3String.Append("#   BlockCheck(0,-1,0,WOOL:0)      Require white wool just below the object\r\n");
                                BO3String.Append("#   BlockCheckNot(0,-1,0,WOOL:0)   Require that there is no white wool below the object\r\n");
                                BO3String.Append("#   LightCheck(0,0,0,0,1)          Require almost complete darkness just below the object\r\n");
                                BO3String.Append("\r\n");
                                if(useBranches)
                                {
                                    BO3String.Append("#######################################################################\r\n");
                                    BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                    BO3String.Append("# |                             Branches                            | #\r\n");
                                    BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                    BO3String.Append("#######################################################################\r\n");
                                    BO3String.Append("\r\n");
                                    BO3String.Append("# Branches are objects that will spawn when this object spawns when it is used in\r\n");
                                    BO3String.Append("# the CustomStructure resource. Branches can also have branches, making complex\r\n");
                                    BO3String.Append("# structures possible. See the wiki for more details.\r\n");
                                    BO3String.Append("\r\n");
                                    BO3String.Append("# Regular Branches spawn each branch with an independent chance of spawning.\r\n");
                                    BO3String.Append("# Branch(x,y,z,branchName,rotation,chance[,anotherBranchName,rotation,chance[,...]][IndividualChance])\r\n");
                                    BO3String.Append("# branchName - name of the object to spawn.\r\n");
                                    BO3String.Append("# rotation - NORTH, SOUTH, EAST or WEST.\r\n");
                                    BO3String.Append("# IndividualChance - The chance each branch has to spawn, assumed to be 100 when left blank\r\n");
                                    BO3String.Append("\r\n");
                                    BO3String.Append("# WeightedBranch(x,y,z,branchName,rotation,chance[,anotherBranchName,rotation,chance[,...]][MaxChanceOutOf])\r\n");
                                    BO3String.Append("# MaxChanceOutOf - The chance all branches have to spawn out of, assumed to be 100 when left blank\r\n");
                                    if (chunk.Key.X == 0 && chunk.Key.Z == 0 && firstPass)
                                    {
                                        BO3String.Append("Branch(0,0,0," + fileName + "C0R0,NORTH,100)\r\n");
                                    }
                                }
                            } else {
                                BO3String.Append("# This BO3 is made for use with MCW and OTG+.\r\n");
                                BO3String.Append("# The descriptions in this file are only accurate for MCW and OTG+.\r\n");
                                BO3String.Append("# For TerrainControl and OTG these descriptions are only accurate if the BO3 is used as a CustomObject.\r\n");
                                BO3String.Append("# MCW and OTG+ completely reimplemented CustomStructure, check the MCW/OTG+ documentation for more information.\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# This is the config file of a custom object.\r\n");
                                BO3String.Append("# If you add this object correctly to your BiomeConfigs, it will spawn in the world.\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# This is the creator of this BO3 object\r\n");
                                BO3String.Append("Author: Unknown\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# A short description of this BO3 object\r\n");
                                BO3String.Append("Description: No description\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# The BO3 version, don't change this! It can be used by external applications to do a version check.\r\n");
                                BO3String.Append("Version: MCW104\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# The settings mode, WriteAll, WriteWithoutComments or WriteDisable. See WorldConfig.\r\n");
                                BO3String.Append("SettingsMode: WriteDisable\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("#######################################################################\r\n");
                                BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                BO3String.Append("# |                          Main settings                          | #\r\n");
                                BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                BO3String.Append("#######################################################################\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# This settings is currently unavailable for MCW/OTG+.\r\n");
                                BO3String.Append("Tree: false\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# The rarity of the BO3 from 0 to 100. Each spawn attempt has rarity% chance to succeed when using the CustomObject(...) resource.\r\n");
                                BO3String.Append("# Ignored by MCW/OTG+ for Tree(..), Sapling(..) and CustomStructure(..).\r\n");
                                BO3String.Append("Rarity: 100\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Minimum distance in chunks between structures with the same filename.\r\n");
                                BO3String.Append("Frequency: 0\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# This settings is currently unavailable for MCW/OTG+, you can easily work around this though by creating a master bo3 without blocks that\r\n");
                                BO3String.Append("# has the real starting BO3 as a branch.\r\n");
                                BO3String.Append("# You can then duplicate the master BO3 3 times and give each copy's branch a different rotation.\r\n");
                                BO3String.Append("RotateRandomly: false\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# The spawn height of the BO3 - randomY or highestBlock.\r\n");
                                BO3String.Append("SpawnHeight: highestBlock\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# The height limits for the BO3. Can be combined with randomY to spawn the BO3 at a random height\r\n");
                                BO3String.Append("# between MinHeight and MaxHeight. Can be combined with highestBlock to make sure that the BO3 spawns\r\n");
                                BO3String.Append("# only between MinHeight and MaxHeight.\r\n");
                                BO3String.Append("MinHeight: 0\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("MaxHeight: 256\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# This settings is disabled for MCW/OTG+.\r\n");
                                BO3String.Append("ExcludedBiomes: All\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# To optimise this BO3 for faster loading times you can use Notepad++ to find/replace comments and abbreviate things.\r\n");
                                BO3String.Append("# Notepad++ find/replace with regex:\r\n");
                                BO3String.Append("# Find: ^[#;].* replace to nothing removes all lines starting with # \r\n");
                                BO3String.Append("# Find: ^\\s*$ replace to nothing removes most empty lines \r\n");
                                BO3String.Append("# Notepad++ find/replace without regex:\r\n");
                                BO3String.Append("# *Warning: This will only work for OTG 1.10.2 v21 / OTG 1.11.2 v6 or higher. B( is not recognised by lower versions.\r\n");
                                BO3String.Append("# Find: Block( replace to B(\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("#######################################################################\r\n");
                                BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                BO3String.Append("# |                      Source block settings                      | #\r\n");
                                BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                BO3String.Append("#######################################################################\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# This settings is disabled for MCW/OTG+.\r\n");
                                BO3String.Append("SourceBlocks: AIR\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# This settings is disabled for MCW/OTG+.\r\n");
                                BO3String.Append("MaxPercentageOutsideSourceBlock: 100\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# This settings is disabled for MCW/OTG+.\r\n");
                                BO3String.Append("OutsideSourceBlock: placeAnyway\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# MineCraft Worlds mod settings #\r\n");
                                BO3String.Append("# NOTE: you can delete any of these settings if you're not using them, the default values will automatically be used. #\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Copies the blocks and branches of an existing BO3 into this BO3.\r\n");
                                if (chunk.Key.X == 0 && chunk.Key.Z == 0 && firstPass)
                                {
                                    BO3String.Append("InheritBO3: " + fileName + "C0R0\r\n");
                                } else {
                                    BO3String.Append("#InheritBO3:\r\n");
                                }
                                BO3String.Append("\r\n");
                                BO3String.Append("# Defaults to true, if true and this is the root BO3 for this branching structure then this BO3's smoothing and height settings are used for all its children (branches).\r\n");
                                BO3String.Append("#OverrideChildSettings: true\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# If this is set to true then this BO3 can spawn as a branch on top of or inside an existing BO3/branch (only within the same customstructure).\r\n");
                                BO3String.Append("#CanOverride: false\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# If this is set to true then this BO3 can only spawn underneath an existing BO3. Used to make sure that dungeons only appear underneath buildings\r\n");
                                BO3String.Append("#MustBeBelowOther: false\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# A comma-seperated list of BO3s, this BO3 must touch one of these when spawned or it will not be able to spawn.\r\n");
                                BO3String.Append("#MustBeInside: \r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# A comma-seperated list of BO3s, this BO3 cannot touch one of these when spawned, if it does it will not be able to spawn.\r\n");
                                BO3String.Append("#CannotBeInside: \r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Defaults to true. Set to false if the BO3 is not allowed to spawn on a water block\r\n");
                                BO3String.Append("#CanSpawnOnWater: true\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Defaults to false. Set to true if the BO3 is allowed to spawn only on a water block\r\n");
                                BO3String.Append("#SpawnOnWaterOnly: false\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Defaults to false. Set to true if the BO3 and its smoothing area should ignore water when looking for the highest block to spawn on. Defaults to false (things spawn on top of water)\r\n");
                                BO3String.Append("#SpawnUnderWater: false\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Defaults to false. Set to true if the BO3 should spawn at water level\r\n");
                                BO3String.Append("#SpawnAtWaterLevel: false\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Spawns the BO3 at a Y offset of this value. Handy when using highestBlock for lowering BO3s into the surrounding terrain when there are layers of ground included in the BO3, also handy when using SpawnAtWaterLevel to lower objects like ships into the water.\r\n");
                                BO3String.Append("#HeightOffset: 0\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# If set to true removes all AIR blocks from the BO3 so that it can be flooded or buried.\r\n");
                                BO3String.Append("#RemoveAir: true\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Replaces all the non-air blocks that are above this BO3 or its smoothing area with the given block material (should be WATER or AIR or NONE), also applies to smoothing areas although it intentionally leaves some of the terrain above them intact. WATER can be used in combination with SpawnUnderWater to fill any air blocks underneath waterlevel with water (and any above waterlevel with air).\r\n");
                                BO3String.Append("#ReplaceAbove: AIR\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Replaces all air blocks underneath the BO3 (but not its smoothing area) with the designated material until a solid block is found.\r\n");
                                BO3String.Append("#ReplaceBelow:\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Defaults to true. If set to true then every block in the BO3 of the materials defined in ReplaceWithGroundBlock or ReplaceWithSurfaceBlock will be replaced by the GroundBlock or SurfaceBlock materials configured for the biome the block is spawned in.\r\n");
                                BO3String.Append("#ReplaceWithBiomeBlocks: true\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Defaults to DIRT, Replaces all the blocks of the given material in the BO3 with the GroundBlock configured for the biome it spawns in\r\n");
                                BO3String.Append("#ReplaceWithGroundBlock: DIRT\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Defaults to GRASS, Replaces all the blocks of the given material in the BO3 with the SurfaceBlock configured for the biome it spawns in\r\n");
                                BO3String.Append("#ReplaceWithSurfaceBlock: GRASS\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Makes the terrain around the BO3 slope gradually towards the edges of the BO3. The given value is the distance in blocks around the BO3 from where the slope should start and can be any positive number.\r\n");
                                BO3String.Append("#SmoothRadius: 0\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Moves the smoothing area up or down relative to the BO3 (at the points where the smoothing area is connected to the BO3). Handy when using SmoothStartTop: false and the BO3 has some layers of ground included, in that case we can set the HeightOffset to a negative value to lower the BO3 into the ground and we can set the SmoothHeightOffset to a positive value to move the smoothing area starting height up\r\n");
                                BO3String.Append("#SmoothHeightOffset: 0\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Should the smoothing area be attached at the bottom or the top of the edges of the BO3? Defaults to false (bottom). Using this setting can make things slower so try to avoid using it and use SmoothHeightOffset instead if for instance you have a BO3 with some ground layers included. The only reason you should need to use this setting is if you have a BO3 with edges that have an irregular height (like some hills).\r\n");
                                BO3String.Append("#SmoothStartTop: false\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Should the smoothing area attach itself to \"log\" block or ignore them? Defaults to false (ignore logs).\r\n");
                                BO3String.Append("#SmoothStartWood: false\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# The block used for smoothing area surface blocks, defaults to biome SurfaceBlock\r\n");
                                BO3String.Append("#SmoothingSurfaceBlock:\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# The block used for smoothing area ground blocks, defaults to biome GroundBlock\r\n");
                                BO3String.Append("#SmoothingGroundBlock:\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Define groups that this BO3 belongs to along with a minimum range in chunks that this BO3 must have between it and any other members of this group if it is to be allowed to spawn. Syntax is \"GroupName:Frequency, GoupName2:Frequency2\" etc so for example a BO3 that belongs to 3 groups: \"BO3Group: Ships:10, Vehicles:5, FloatingThings:3\"\r\n");
                                BO3String.Append("#BO3Group:\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Defaults to false. Set to true if this BO3 should spawn at the player spawn point. When the server starts the spawn point is determined and the BO3's for the biome it is in are loaded, one of these BO3s that has IsSpawnPoint set to true (if any) is selected randomly and is spawned at the spawn point regardless of its rarity (so even Rarity:0, IsSpawnPoint: true BO3's can get spawned as the spawn point!).\r\n");
                                BO3String.Append("#IsSpawnPoint: false\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Defaults to 8, if SpawnPointY > -1 and this BO3 is spawned at the spawn point then the player spawn point is placed within this BO3 at x y and z offsets (spawnPointX, spawnPointY, spawnPointZ) relative to x0y0z0 in this BO3.\r\n");
                                BO3String.Append("#SpawnPointX: 8\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Defaults to highestBlock, if this is defined and this BO3 is spawned at the spawn point then the player spawn point is placed at a y offset of this value relative to y = 0 in this BO3.)\r\n");
                                BO3String.Append("#SpawnPointY: -1\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Defaults to 7, if spawnPointY > -1 and this BO3 is spawned at the spawn point then the player spawn point is placed within this BO3 at a x y and z offsets (spawnPointX, spawnPointY, spawnPointZ) relative to x0y0z0 in this BO3.\r\n");
                                BO3String.Append("#SpawnPointZ: 7\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("#######################################################################\r\n");
                                BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                BO3String.Append("# |                              Blocks                             | #\r\n");
                                BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                BO3String.Append("#######################################################################\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# All the blocks used in the BO3 are listed here. Possible blocks:\r\n");
                                BO3String.Append("# Block(x,y,z,id[.data][,nbtfile.nbt) or B(x,y,z,id[.data][,nbtfile.nbt)\r\n");
                                BO3String.Append("# RandomBlock(x,y,z,id[:data][,nbtfile.nbt],chance[,id[:data][,nbtfile.nbt],chance[,...]])\r\n");
                                BO3String.Append("# So RandomBlock(0,0,0,CHEST,chest.nbt,50,CHEST,anotherchest.nbt,100) will spawn a chest at\r\n");
                                BO3String.Append("# the BO3 origin, and give it a 50% chance to have the contents of chest.nbt, or, if that\r\n");
                                BO3String.Append("# fails, a 100% percent chance to have the contents of anotherchest.nbt.\r\n");
                                BO3String.Append("\r\n");
                                if (chunk.Key.X != 0 || chunk.Key.Z != 0 || !firstPass)
                                {
                                    BO3String.Append(chunk.Value);
                                    if (chunk.Value.Length > 0)
                                    {
                                        BO3String.Append("\r\n");
                                    }
                                }
                                BO3String.Append("\r\n");
                                BO3String.Append("#######################################################################\r\n");
                                BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                BO3String.Append("# |                             ModData                             | #\r\n");
                                BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                BO3String.Append("#######################################################################\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Currently only available for MCW, this feature will be added to OTG+ a.s.a.p.");
                                BO3String.Append("# Use the ModData() tag to include data that other mods can use\r\n");
                                BO3String.Append("# ModData(x,y,z,ModName,DataText)\r\n");
                                BO3String.Append("# Example: ModData(x,y,z,MyCystomNPCMod,SpawnBobHere/WithAPotato/And50Health)\r\n");
                                BO3String.Append("# MCW has some built in ModData commands for basic mob and block spawning.\r\n");
                                BO3String.Append("# These are mostly just a demonstration for mod makers to show how ModData.\r\n");
                                BO3String.Append("# can be used by other mods.\r\n");
                                BO3String.Append("# For mob spawning in MCW use: ModData(x,y,z,TC/MCW,mob/MobType/Count/Persistent/Name)\r\n");
                                BO3String.Append("# mob: Makes MCW recognise this as a mob spawning command.\r\n");
                                BO3String.Append("# MobType: Lower-case, no spaces. Any vanilla mob like dragon, skeleton, wither, villager etc\r\n");
                                BO3String.Append("# Count: The number of mobs to spawn\r\n");
                                BO3String.Append("# Persistent (true/false): Should the mobs never de-spawn? If set to true the mob will get a\r\n");
                                BO3String.Append("# name-tag ingame so you can recognise it.\r\n");
                                BO3String.Append("# Name: A name-tag for the monster/npc.\r\n");
                                BO3String.Append("# Example: ModData(0,0,0,TC/MCW,villager/1/true/Bob)\r\n");
                                BO3String.Append("# For a complete list of possible creatures check the mc forums or the mctcp forums.\r\n");
                                BO3String.Append("# To spawn blocks using ModData use: ModData(x,y,z,TC/MCW,block/material)\r\n");
                                BO3String.Append("# block: Makes MCW recognise this as a block spawning command.\r\n");
                                BO3String.Append("# material: id or text, custom blocks can be added using ModName:MaterialName.\r\n");
                                BO3String.Append("# To send all ModData within a radius in chunks around the player to the specified mod\r\n");
                                BO3String.Append("# use this console command: /mcw GetModData ModName Radius\r\n");
                                BO3String.Append("# ModName: name of the mod, for MCW commands use TC/MCW \r\n");
                                BO3String.Append("# Radius (optional): Radius in chunks around the player.\r\n");
                                BO3String.Append("# Mod makers can use ModData and the /mcw GetModData command to test IMC communications between MCW\r\n");
                                BO3String.Append("# and their mod.\r\n");
                                BO3String.Append("# Normal users can use it to spawn some mobs and blocks on command!\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("#######################################################################\r\n");
                                BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                BO3String.Append("# |                             Entities                            | #\r\n");
                                BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                BO3String.Append("#######################################################################\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Entity() spawns an entity instead of a block. The entity is spawned only once when the BO3 is spawned.\r\n");
                                BO3String.Append("# Entities are persistent by default so they don't de-spawn when no player is near, they are only unloaded.\r\n");
                                BO3String.Append("# Usage: Entity(x,y,z,mobName,groupSize,NameTagOrNBTFileName) or Entity(x,y,z,mobName,groupSize)\r\n");
                                BO3String.Append("# Use /tc entities or /otg entities to get a list of entities that can be used as mobName, this includes entities added by other mods.\r\n");
                                BO3String.Append("# NameTagOrNBTFileName can be either a nametag for the mob or an nbt file (such as mymobinfo.nbt or mymobinfo.txt).\r\n");
                                BO3String.Append("# When using a text file you can use the same mob spawning parameters used with the /summon command to equip the\r\n");
                                BO3String.Append("# entity and give it custom attributes etc. You can copy the DATA part of a summon command including surrounding\r\n");
                                BO3String.Append("# curly braces to a .txt file, for instance for: \"/summon Skeleton x y z {DATA}\"\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("#######################################################################\r\n");
                                BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                BO3String.Append("# |                             Spawners                            | #\r\n");
                                BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                BO3String.Append("#######################################################################\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Currently only available for MCW, this feature will be added to OTG+ a.s.a.p.");
                                BO3String.Append("# Spawner(x,y,z,EntityType,count,interval,chance,max,despawnTime,velocityX,velocityY,velocityZ,yaw,pitch) tag for BO3's,\r\n");
                                BO3String.Append("# works just like the Block() tag and is spawned when the BO3 is spawned. Spawners are invisible ingame but can be blocked\r\n");
                                BO3String.Append("# up with a solid or liquid block (depending on if the entity being spawned can spawn in those blocks) to prevent entities\r\n");
                                BO3String.Append("# from spawning. A spawner tries to spawn <count> number of <entityType> each <interval> seconds, with each entity having\r\n");
                                BO3String.Append("# a spawn chance of <chance>%. The spawner can only spawn an entity if there are not more than <max> of the same EntityType\r\n");
                                BO3String.Append("# in the surrounding chunks (3x3 chunk area centered on the spawner). If an entity is assigned a <despawnTime> in seconds\r\n");
                                BO3String.Append("# then the entity will automatically be despawned after the specified time. Entities spawned using a Spawner will have the\r\n");
                                BO3String.Append("# same light level requirements as they do when they normally spawn in the world, so most monsters only spawn in the dark.\r\n");
                                BO3String.Append("# A spawner will not spawn if the spawn location is blocked by another entity, it can however spawn multiple mobs at once.\r\n");
                                BO3String.Append("# The last 6 parameters are optional and are set to 0 by default (despawnTime,velocityX,velocityY,velocityZ,yaw,pitch), you\r\n");
                                BO3String.Append("# only need to add as many of those parameters as you need. So for instance all of these work: \"Spawner(0,0,0,Arrow,1,5,100,5)\"\r\n");
                                BO3String.Append("# \"Spawner(0,0,0,Arrow,1,5,100,5, 1,0,1)\" \"Spawner(0,0,0,Arrow,1,5,100,5,1,0,1,180,180)\"\r\n");
                                BO3String.Append("# Use the /mcw entities or /otg entities console command (ops only) to see a list of available entityTypes.\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("#######################################################################\r\n");
                                BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                BO3String.Append("# |                             Particles                           | #\r\n");
                                BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                BO3String.Append("#######################################################################\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Currently only available for MCW, this feature will be added to OTG+ a.s.a.p.");
                                BO3String.Append("# Continually spawns particles in the world at the specified coordinates, interval and velocity.\r\n");
                                BO3String.Append("# For a list of particles use /mcw particles or /otg particles.\r\n");
                                BO3String.Append("# When no velocity is supplied a small random velocity is used.\r\n");
                                BO3String.Append("# Particles will disappear if a solid block is placed on them for at least one second.\r\n");
                                BO3String.Append("# Usage1: Particle(x, y , z, particleName, interval\r\n");
                                BO3String.Append("# Usage2: Particle(x, y , z, particleName, interval, velocityX, velocityY, velocityZ)\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("#######################################################################\r\n");
                                BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                BO3String.Append("# |                            BO3 checks                           | #\r\n");
                                BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                BO3String.Append("#######################################################################\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# This settings is disabled for MCW/OTG+.\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("#######################################################################\r\n");
                                BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                BO3String.Append("# |                             Branches                            | #\r\n");
                                BO3String.Append("# +-----------------------------------------------------------------+ #\r\n");
                                BO3String.Append("#######################################################################\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# Check the otg docs, mctcp forums, the mc forums and PeeGee85 YT channel for.\r\n");
                                BO3String.Append("# Documentation and tutorials for the branching features.\r\n");
                                BO3String.Append("# Branch(x,y,z,isBranchEnding,branchName,rotation,chance,branchLength)\r\n");
                                BO3String.Append("# branchName - filename of the object to spawn (without extension).\r\n");
                                BO3String.Append("# rotation - NORTH, SOUTH, EAST or WEST.\r\n");
                                BO3String.Append("# chance - Chance to spawn between 0 and 100\r\n");
                                BO3String.Append("\r\n");
                                BO3String.Append("# WeightedBranch(x,y,z,isBranchEnding,branchName,rotation,chance,branchLength,[anotherBranchName,rotation,chance,branchLength[,...]],maxChanceOutOf)\r\n");
                                BO3String.Append("# maxChanceOutOf - The chance all branches have to spawn out of, assumed to be 100 when left blank\r\n");
                                BO3String.Append("# Example: WeightedBranch(0,0,0,true,RandomInterior1,NORTH,20,0,RandomInterior2,NORTH,20,0,RandomInterior3,NORTH,20,0,RandomInterior4,NORTH,20,0)\r\n");
                                BO3String.Append("\r\n");
                            }

                            if ((chunk.Key.X != 0 || chunk.Key.Z != 0 || !firstPass) && blocksPerChunkArr[chunk.Key.X, chunk.Key.Z] != null)
                            {
                                if (exportForTC)
                                {
                                    BO3String.Append("Branch(16,0,0," + fileName + "C" + (chunk.Key.X + 1) + "R" + chunk.Key.Z + ",NORTH,100)\r\n");
                                } else {
                                    BO3String.Append("Branch(16,0,0,true," + fileName + "C" + (chunk.Key.X + 1) + "R" + chunk.Key.Z + ",NORTH,100,0)\r\n");
                                }
                            }
                            if ((chunk.Key.X != 0 || chunk.Key.Z != 0 || !firstPass) && chunk.Key.X == 0)
                            {
                                if (blocksPerChunkArr[chunk.Key.X, chunk.Key.Z] != null)
                                {
                                    if (exportForTC)
                                    {
                                        BO3String.Append("Branch(0,0,16," + fileName + "C" + chunk.Key.X + "R" + (chunk.Key.Z + 1) + ",NORTH,100)\r\n");
                                    } else {
                                        BO3String.Append("Branch(0,0,16,true," + fileName + "C" + chunk.Key.X + "R" + (chunk.Key.Z + 1) + ",NORTH,100,0)\r\n");
                                    }
                                }
                            }

                            if (!useBranches || (chunk.Key.X == 0 && chunk.Key.Z == 0 && firstPass))
                            {
                                using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter(saveDir.FullName + "\\" + fileName + ".BO3"))
                                {
                                    outputFile.Write(BO3String);
                                }
                            } else {
                                if (!saveSubDirCreated)
                                {
                                    System.IO.DirectoryInfo saveSubDir = new System.IO.DirectoryInfo(saveDir.FullName + "\\" + fileName);

                                    if (!saveSubDir.Exists)
                                    {
                                        saveSubDir.Create();
                                        saveSubDir.Refresh();

                                        System.Security.AccessControl.DirectorySecurity sec3 = saveSubDir.GetAccessControl();
                                        System.Security.AccessControl.FileSystemAccessRule accRule3 = new System.Security.AccessControl.FileSystemAccessRule(Environment.UserDomainName + "\\" + Environment.UserName, System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.AccessControlType.Allow);
                                        sec3.AddAccessRule(accRule3);
                                    }
                                }
                                using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter(saveDir.FullName + "\\" + fileName + "\\" + fileName + "C" + chunk.Key.X + "R" + chunk.Key.Z + ".BO3"))
                                {
                                    outputFile.Write(BO3String);
                                }
                            }
                            if ((chunk.Key.X == 0 && chunk.Key.Z == 0 && firstPass))
                            {
                                firstPass = false;
                                i--;
                            }
                        }
                    }
                }
            }
        }
    }
}
