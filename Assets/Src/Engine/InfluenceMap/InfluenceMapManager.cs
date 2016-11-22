﻿using UnityEngine;

/*
My Influence -- All Influence coming from my units,buildings etc
Opponent Influence -- All influence coming from opposing units,buildings etc
Influence map -- Calculated as My Influence-Opponent Influence
Tension map -- Calculated as My Influence+OpponentInfluence
Vulnerability Map -- Calculated as Tension map -Abs(Influence map)

different maps mean different things
for positional map
    multiply impassable ndoes by 0
    for friendly position
        //gather allies within relavant range
        //stamp influence
    for hostile position
        //gather hostile within range
        //stamp influence
    for grenade position
        //merge friendly + hostile position
        //find area with highest enemy influence
        //if concentration > some threshold -> throw


Limit ai units area of interest
tactical path finding -- use node system (not required to be a grid), pick position based on cover etc
*/

public class InfluenceMapManager : MonoBehaviour {

    private Texture2D tex;
    public AnimationCurve curve;
    private bool texIsDirty;

    [Header("Set these from the terrain object")]
    public int terrainOriginOffset = 128;
    public int terrainResolution = 256;

    [Header("Should match terrain resolution for 1 - 1 cell size")]
    public int dimension = 128;

    public static InfluenceMapManager Instance;
    Color[] pixels;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        //float terrainSizeRatio = terrainResolution / dimension;
        pixels = new Color[(dimension * dimension)];
        for (int i = 0; i < pixels.Length; i++) {
            //int x = i % (dimension);
            //int z = i / (dimension);

            //int positionX = (int)(terrainSizeRatio * x) - terrainOriginOffset;
            //int positionZ = (int)(terrainSizeRatio * z) - terrainOriginOffset;

            //Vector3 position = new Vector3(positionX + 1f, 0, positionZ + 1f);
            //var p = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //p.transform.position = position;
            //p.isStatic = true;
            //p.name = "(" + x + ", " + z + ")";
            Color color = new Color(0, 0, 0, 0);
            pixels[i] = color;
        }

        tex = new Texture2D(dimension, dimension, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.SetPixels(pixels);
        tex.Apply();
    }

    void Start() {
        GetComponent<Projector>().material.SetTexture("_ShadowTex", tex);
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Ray worldRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(worldRay, out hit)) {
                //Vector3 worldPoint = hit.point;
                //UpdatePhysicalInfluence(worldPoint, 3);
            }
        }
        if (texIsDirty) {
            tex.Apply();
        }
    }

    public InfluenceMapSection UpdatePhysicalInfluence(Vector3 position, InfluenceMapSection section, int distance = 3) {
        texIsDirty = true;
        //todo store grid outside of texture, use texture for display only
        for (int i = 0; i < section.length; i++) {
            var node = section.nodes[i];
            int pixelIndex = node.col * dimension + node.row;
            Color pixelColor = pixels[pixelIndex];
            pixels[pixelIndex] = pixelColor - new Color(0, 1, 0, node.influence); 
            tex.SetPixel(node.col, node.row, pixels[pixelIndex]);
        }
        float percentX = (position.x + terrainOriginOffset) / terrainResolution;
        float percentZ = (position.z + terrainOriginOffset) / terrainResolution;

        int row = (int)(percentZ * dimension);
        int col = (int)(percentX * dimension);

        int r = row - distance;
        int c = col - distance;
        int count = distance * 2 + 1;
        section.Clear();

        int mapIdx = 0;
        for (int i = 0; i < count; i++) {
            for (int j = 0; j < count; j++) {

                int ci = c + i;
                int rj = r + j;

                if (ci >= 0 && ci < dimension && rj >= 0 && rj < dimension) {
                    int pixelIndex = ci * dimension + rj;
                    float normalizedDistance = 1 - ((new Vector2(col, row) - new Vector2(ci, rj)).magnitude / 5f);
                    float influence = curve.Evaluate(normalizedDistance);

                    section.nodes[mapIdx++] = new InfluenceMapNode(ci, rj, influence);

                    Color color = pixels[pixelIndex] + new Color(0, 1, 0, influence);
                    tex.SetPixel(ci, rj, color);
                    pixels[pixelIndex] = color;
                }
            }
        }
        section.length = mapIdx;
        tex.SetPixel(col, row, new Color(1, 0, 0, 1));
        return section;
    }

    public float EvalCurve(float input, float slope, float exp, float vShift, float hShift) {
        input = Mathf.Clamp01(input);
        float y = slope * Mathf.Pow((input - hShift), exp) + vShift;
        float dem = (1000 * slope * (float)System.Math.E);
        dem = Mathf.Pow(dem, -1 * input + hShift) + 1;

       // float logisticY = exp * (1 / dem) + vShift;
        return Mathf.Clamp01(y);
    }

    public float LinearCurve(float input) {
        return Mathf.Clamp01((1f - Mathf.Clamp01(input)));
    }

    public float ExponentialCurve(float input) {
        return Mathf.Clamp01(1 - Mathf.Pow(Mathf.Clamp01(input), 3));
    }
}

