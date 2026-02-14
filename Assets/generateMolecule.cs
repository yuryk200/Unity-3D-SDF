using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class generateMolecule : MonoBehaviour
{
    public GameObject[] atomPrefabs;
    public Material[] atomMaterials;
    public float bondRadius = 0.05f;
    List<Atom> atoms = new List<Atom>();
    List<Bond> bonds = new List<Bond>();
    private readonly List<GameObject> spawned = new List<GameObject>();
    private string _lastLoadedPath = null;


    class Atom
    {
        public Vector3 position;
        public string element;
        public int atomNumber;
        public Atom(Vector3 position, string element, int atomNumber)
        {
            this.position = position;
            this.element = element;
            this.atomNumber = atomNumber;
        }
    }

    class Bond
    {
        public int atomIndex1;
        public int atomIndex2;
        public int bondOrder;
        public Bond(int atomIndex1, int atomIndex2, int bondOrder)
        {
            this.atomIndex1 = atomIndex1;
            this.atomIndex2 = atomIndex2;
            this.bondOrder = bondOrder;
        }
    }

    public void LoadSdfFromPath(string absolutePath)
    {
        Debug.Log("LoadSdfFromPath called with: " + absolutePath);

        if (string.IsNullOrWhiteSpace(absolutePath) || !File.Exists(absolutePath))
        {
            Debug.LogError("SDF file not found: " + absolutePath);
            return;
        }

        _lastLoadedPath = absolutePath;

        ClearMolecule();
        StartBond(absolutePath);
        makebond(absolutePath);
    }

    private void ClearMolecule()
    {
        atoms.Clear();
        bonds.Clear();

        // Destroy previously spawned objects
        foreach (var go in spawned)
        {
            if (go != null) Destroy(go);
        }
        spawned.Clear();
    }


    void StartBond(string path)
    {
        _lastLoadedPath = path;

        using var reader = new StreamReader(path);


        // Skip lines until "M  V30 BEGIN ATOM" is found
        while (!reader.EndOfStream && !reader.ReadLine().StartsWith("M  V30 BEGIN ATOM"))
        {
            // do nothing
        }

        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();

            if (line.StartsWith("M  V30"))
            {
                // Parse atom data
                if (line.StartsWith("M  V30 END ATOM"))
                {
                    // Exit loop after parsing bond data
                    makebond();
                    break;
                }
                int atomNumber;
                if (!int.TryParse(line.Substring(7, 2).Trim(), out atomNumber))
                {
                    Debug.LogError("Failed to parse atom number: " + line.Substring(7, 2).Trim());
                    continue;
                }
                Debug.Log(atomNumber);

                string element = line.Substring(9, 2).Trim();
                Debug.Log("Element: " + element);

                int shift1 = 11;
                int shift2 = 20;
                int shift3 = 29;
                int w = 9;

                if (line.StartsWith("M  V30 ") && line.Substring(7, 2) != "00")
                {
                    int lineNumber;
                    if (!int.TryParse(line.Substring(7, 2), out lineNumber))
                    {
                        Debug.LogError("Failed to parse line number: " + line.Substring(7, 2));
                        continue;
                    }

                    if (lineNumber >= 10 && lineNumber <= 40)
                    {
                        shift1 = 12;
                        shift2 = 21;
                        shift3 = 30;

                        string check = line.Substring(12, 1).Trim();
                        Debug.Log("check if " + check);

                        if (check == "-")
                        {
                            w = 10;
                            shift2 = 22;
                            shift3 = 31;

                            string check2 = line.Substring(22, 1).Trim();
                            Debug.Log("check if " + check2);
                            if (check2 == "-")
                            {
                                shift3 = 32;
                                string check3 = line.Substring(32, 1).Trim();
                                if (check3 != "-")
                                {
                                    w = 9;
                                }
                            }
                            else if (check2 != "-")
                            {
                                w = 9;
                            }
                        }
                        else if (check != "-")
                        {
                            string check2 = line.Substring(22, 1).Trim();
                            Debug.Log("check if " + check2);
                            if (check2 == "-")
                            {
                                w = 10;
                                shift3 = 32;
                                string check3 = line.Substring(32, 1).Trim();
                                if (check3 != "-")
                                {
                                    w = 9;
                                }
                            }
                            else if (check2 != "-")
                            {
                                w = 9;
                            }
                        }
                    }

                    if (lineNumber >= 1 && lineNumber <= 9)
                    {
                        shift1 = 11;
                        shift2 = 20;
                        shift3 = 29;

                        string check = line.Substring(11, 1).Trim();
                        Debug.Log("check if " + check);

                        if (check == "-")
                        {
                            w = 10;
                            shift2 = 21;
                            shift3 = 30;

                            string check2 = line.Substring(21, 1).Trim();
                            Debug.Log("check if " + check2);
                            if (check2 == "-")
                            {
                                shift3 = 31;
                                string check3 = line.Substring(31, 1).Trim();
                                if (check3 != "-")
                                {
                                    w = 9;
                                }
                            }
                            else if (check2 != "-")
                            {
                                w = 9;
                            }
                        }
                        else if (check != "-")
                        {
                            string check2 = line.Substring(21, 1).Trim();
                            Debug.Log("check if " + check2);
                            if (check2 == "-")
                            {
                                w = 10;
                                shift3 = 32;
                                string check3 = line.Substring(31, 1).Trim();
                                if (check3 != "-")
                                {
                                    w = 9;
                                }
                            }
                            else if (check2 != "-")
                            {
                                w = 9;
                            }
                        }
                    }
                }

                float x;
                if (!float.TryParse(line.Substring(shift1, w).Trim(), out x))
                {
                    Debug.LogError("x: " + line.Substring(shift1, w).Trim());
                    continue;
                }
                Debug.Log(x);

                float y;
                if (!float.TryParse(line.Substring(shift2, w).Trim(), out y))
                {
                    Debug.LogError("y: " + line.Substring(shift2, w).Trim());
                    continue;
                }
                Debug.Log(y);

                float z;
                if (!float.TryParse(line.Substring(shift3, w).Trim(), out z))
                {
                    Debug.LogError("z: " + line.Substring(shift3, w).Trim());
                    continue;
                }
                Debug.Log(z);

                Vector3 position = new Vector3(x, y, z);

                atoms.Add(new Atom(position, element, atomNumber));


            }

        }


        GameObject[] atomObjects = new GameObject[atoms.Count];

        for (int i = 0; i < atoms.Count; i++)
        {
            int elementIndex = ElementToIndex(atoms[i].element);
            if (elementIndex < 0)
            {
                Debug.LogWarning("Unsupported element: " + atoms[i].element + " (skipping render)");
                continue;
            }

            var atomObject = Instantiate(atomPrefabs[elementIndex], atoms[i].position, Quaternion.identity);
            atomObject.GetComponent<Renderer>().material = atomMaterials[elementIndex];
            atomObject.transform.SetParent(transform);
            spawned.Add(atomObject);
        }


    }

    void makebond()
    {
        if (string.IsNullOrWhiteSpace(_lastLoadedPath))
        {
            Debug.LogError("makebond(): No SDF path has been loaded yet.");
            return;
        }
        makebond(_lastLoadedPath);
    }

    void makebond(string path)
    {
        using var reader = new StreamReader(path);

        while (!reader.EndOfStream && !reader.ReadLine().StartsWith("M  V30 BEGIN BOND"))
        {
            // do nothing
        }
        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();

            int bshift1 = 11;
            int bshift2 = 13;
            int bshift3 = 9;
            int w1 = 1;
            int w2 = 1;

            if (line.StartsWith("M  V30"))
            {
                if (line.StartsWith("M  V30 END BOND"))
                {
                    // Exit loop after parsing bond data
                    CreateBonds();
                    break;
                }

                if (line.StartsWith("M  V30 ") && line.Substring(7, 2) != "00")
                {
                    int lineNumber;
                    if (!int.TryParse(line.Substring(7, 2), out lineNumber))
                    {
                        Debug.LogError("Failed to parse line number: " + line.Substring(7, 2));
                        continue;
                    }

                    if (lineNumber >= 10 && lineNumber <= 40)
                    {
                        bshift1 = 12;
                        bshift2 = 14;
                        bshift3 = 10;

                        int checkline = 0;
                        if (!int.TryParse(line.Substring(bshift1, 2), out checkline))
                        {
                            Debug.LogError("Failed to parse line number: " + line.Substring(bshift1, 2));
                            continue;
                        }

                        if (checkline >= 10 && checkline <= 40)
                        {
                            bshift2 = 15;
                            w1 = 2;
                        }

                        if (line.Length >= bshift2 + 2)
                        {
                            int checkline2;
                            if (int.TryParse(line.Substring(bshift2, 2), out checkline2))
                            {
                                w2 = 2;
                            }
                            else if (int.TryParse(line.Substring(bshift2, 1), out checkline2))
                            {
                                w2 = 1;
                            }
                            else
                            {
                                Debug.LogError("Failed to parse line number: " + line.Substring(bshift2));
                                continue;
                            }
                        }
                    }

                    if (lineNumber >= 1 && lineNumber <= 9)
                    {
                        bshift1 = 11;
                        bshift2 = 13;
                        bshift3 = 9;

                        int checkline = 0;
                        if (!int.TryParse(line.Substring(bshift1, 2), out checkline))
                        {
                            Debug.LogError("Failed to parse line number: " + line.Substring(bshift1, 2));
                            continue;
                        }

                        if (checkline >= 10 && checkline <= 40)
                        {
                            bshift2 = 15;
                            w1 = 2;
                        }

                        if (line.Length >= bshift2 + 2)
                        {
                            int checkline2;
                            if (int.TryParse(line.Substring(bshift2, 2), out checkline2))
                            {
                                w2 = 2;
                            }
                            else if (int.TryParse(line.Substring(bshift2, 1), out checkline2))
                            {
                                w2 = 1;
                            }
                            else
                            {
                                Debug.LogError("Failed to parse line number: " + line.Substring(bshift2));
                                continue;
                            }
                        }
                    }
                }

                int atomIndex1;
                if (!int.TryParse(line.Substring(bshift1, w1).Trim(), out atomIndex1))
                {
                    Debug.LogError("atom 1: " + line.Substring(bshift1, w1).Trim());
                    continue;
                }
                Debug.Log("atom 1: " + atomIndex1);

                int atomIndex2;
                if (!int.TryParse(line.Substring(bshift2, w2).Trim(), out atomIndex2))
                {
                    Debug.LogError("atom 2: " + line.Substring(bshift2, w2).Trim());
                    continue;
                }
                Debug.Log("atom 2: " + atomIndex2);

                int bondOrder;
                if (!int.TryParse(line.Substring(bshift3, 1).Trim(), out bondOrder))
                {
                    Debug.LogError("bond: " + line.Substring(bshift3, 1).Trim());
                    continue;
                }
                Debug.Log("bond: " + bondOrder);

                bonds.Add(new Bond(atomIndex1, atomIndex2, bondOrder));
                
            }
        }
    }

    public void CreateBonds()
    {
        // Loop through all the bonds in the Bond class
        foreach (var bond in bonds)
        {
            Debug.Log("bonds make: " + bond.atomIndex1);
            Debug.Log("bonds make: " + bond.atomIndex2);
            // Get the atoms involved in the bond based on their indices
            Atom atom1 = atoms.Find(atom => atom.atomNumber == bond.atomIndex1);
            Atom atom2 = atoms.Find(atom => atom.atomNumber == bond.atomIndex2);
            Debug.Log("atom1: " + atom1.position);
            Debug.Log("atom2: " + atom2.position);

            // Create the bond object based on the bond order
            GameObject bondObject = null;
            if (bond.bondOrder == 1)
            {
                Vector3 bondDirection = (atom2.position - atom1.position).normalized;
                float bondDistance = Vector3.Distance(atom1.position, atom2.position);

                bondObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                bondObject.name = "Bond";
                bondObject.transform.SetParent(transform);
                bondObject.transform.position = (atom1.position + atom2.position) / 2f;
                bondObject.transform.up = bondDirection;
                bondObject.transform.localScale = new Vector3(bondRadius, bondDistance / 2f, bondRadius);
                bondObject.GetComponent<Renderer>().material = new Material(Shader.Find("Standard"));

                spawned.Add(bondObject);
            }
            else if (bond.bondOrder == 2)
            {
                Vector3 bondDirection = (atom2.position - atom1.position).normalized;
                float bondDistance = Vector3.Distance(atom1.position, atom2.position);
                Vector3 offset = Vector3.Cross(bondDirection, Vector3.up).normalized * bondRadius * 1f;

                Vector3 position1 = (atom1.position + atom2.position) / 2f - offset;
                Vector3 position2 = (atom1.position + atom2.position) / 2f + offset;

                // Bond 1
                bondObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                bondObject.name = "Bond1";
                bondObject.transform.SetParent(transform);
                bondObject.transform.position = position1;
                bondObject.transform.up = bondDirection;
                bondObject.transform.localScale = new Vector3(bondRadius, bondDistance / 2f, bondRadius);
                bondObject.GetComponent<Renderer>().material = new Material(Shader.Find("Standard"));
                spawned.Add(bondObject);

                // Bond 2
                bondObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                bondObject.name = "Bond2";
                bondObject.transform.SetParent(transform);
                bondObject.transform.position = position2;
                bondObject.transform.up = bondDirection;
                bondObject.transform.localScale = new Vector3(bondRadius, bondDistance / 2f, bondRadius);
                bondObject.GetComponent<Renderer>().material = new Material(Shader.Find("Standard"));
                spawned.Add(bondObject); 
            }

            else if (bond.bondOrder == 3)
            {
                Vector3 bondDirection = (atom2.position - atom1.position).normalized;
                float bondDistance = Vector3.Distance(atom1.position, atom2.position);
                Vector3 bondOffset1 = Vector3.Cross(bondDirection, Vector3.up).normalized * bondRadius * 2f;
                Vector3 bondOffset2 = Vector3.Cross(bondDirection, Vector3.forward).normalized * bondRadius * 2f;
                Vector3 bondOffset3 = Vector3.Cross(bondDirection, Vector3.right).normalized * bondRadius * 2f;

                // Create three cylinders to make up the triangular prism
                bondObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                bondObject.name = "Bond";
                bondObject.transform.SetParent(transform);
                bondObject.transform.position = (atom1.position + atom2.position) / 2f + bondOffset1;
                bondObject.transform.up = bondDirection;
                bondObject.transform.localScale = new Vector3(bondRadius, bondDistance / 2f, bondRadius);
                bondObject.GetComponent<Renderer>().material = new Material(Shader.Find("Standard"));
                spawned.Add(bondObject);

                bondObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                bondObject.name = "Bond";
                bondObject.transform.SetParent(transform);
                bondObject.transform.position = (atom1.position + atom2.position) / 2f + bondOffset2;
                bondObject.transform.up = bondDirection;
                bondObject.transform.localScale = new Vector3(bondRadius, bondDistance / 2f, bondRadius);
                bondObject.GetComponent<Renderer>().material = new Material(Shader.Find("Standard"));
                spawned.Add(bondObject);

                bondObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                bondObject.name = "Bond";
                bondObject.transform.SetParent(transform);
                bondObject.transform.position = (atom1.position + atom2.position) / 2f + bondOffset3;
                bondObject.transform.up = bondDirection;
                bondObject.transform.localScale = new Vector3(bondRadius, bondDistance / 2f, bondRadius);
                bondObject.GetComponent<Renderer>().material = new Material(Shader.Find("Standard"));
                spawned.Add(bondObject);
            }
         
        }

    }

    private int ElementToIndex(string element)
    {
        switch (element)
        {
            case "C": return 0;
            case "H": return 1;
            // TODO: add "O", "N", "Cl", etc. once you have prefabs/materials
            default: return -1;
        }
    }

    void Start()
    {
        // Optional: if you want auto-load on scene start, uncomment:
        if (!string.IsNullOrWhiteSpace(_lastLoadedPath) && File.Exists(_lastLoadedPath))
            LoadSdfFromPath(_lastLoadedPath);
    }

    // Helper: lets Android/UnitySendMessage pass the SDF text directly.
    // It writes the text to a file and then reuses your existing pipeline.
    public void LoadSdfFromText(string sdfText)
    {
        if (string.IsNullOrWhiteSpace(sdfText))
        {
            Debug.LogError("LoadSdfFromText: sdfText was empty.");
            return;
        }

        try
        {
            string path = Path.Combine(Application.persistentDataPath, "latest_from_server.sdf");
            File.WriteAllText(path, sdfText);
            Debug.Log("LoadSdfFromText wrote SDF to: " + path);
            LoadSdfFromPath(path);
        }
        catch (Exception e)
        {
            Debug.LogError("LoadSdfFromText failed: " + e);
        }
    }

    // Optional: call this to force a reload of the last file path
    public void ReloadLastSdf()
    {
        if (string.IsNullOrWhiteSpace(_lastLoadedPath))
        {
            Debug.LogError("ReloadLastSdf: no last path stored yet.");
            return;
        }
        LoadSdfFromPath(_lastLoadedPath);
    }

}