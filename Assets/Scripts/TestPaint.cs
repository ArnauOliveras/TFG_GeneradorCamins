using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPaint : MonoBehaviour
{
    public Terrain terrain; // Referencia al terreno que deseas pintar
    public Texture2D textureToPaint; // Textura que deseas pintar en el terreno
    public float brushSize = 5f; // Tamaño del pincel de pintura

    // Pinta la textura en las coordenadas dadas
    public void PaintTexture(Vector3 worldPos)
    {
        // Convertir coordenadas del mundo a coordenadas del terreno
        Vector3 terrainPos = worldPos - terrain.transform.position;
        Vector2 normalizedPos = new Vector2(
            terrainPos.x / terrain.terrainData.size.x,
            terrainPos.z / terrain.terrainData.size.z
        );

        // Convertir coordenadas normalizadas a coordenadas de textura
        int textureX = (int)(normalizedPos.x * terrain.terrainData.alphamapWidth);
        int textureY = (int)(normalizedPos.y * terrain.terrainData.alphamapHeight);

        // Aplicar la textura en un área alrededor de la posición dada según el tamaño del pincel
        float[,,] splatmapData = terrain.terrainData.GetAlphamaps(textureX, textureY, (int)brushSize, (int)brushSize);
        for (int x = 0; x < (int)brushSize; x++)
        {
            for (int y = 0; y < (int)brushSize; y++)
            {
                // Verificar si los índices están dentro de los límites del array
                int indexX = textureX + x;
                int indexY = textureY + y;
                if (indexX >= 0 && indexX < splatmapData.GetLength(0) && indexY >= 0 && indexY < splatmapData.GetLength(1))
                {
                    // Asignar la textura solo si los índices están dentro de los límites
                    splatmapData[indexY, indexX, 0] = 1f; // Asigna 100% de la textura en la capa 0
                }
            }
        }
        terrain.terrainData.SetAlphamaps(textureX, textureY, splatmapData);
    }

    // Método de actualización
    void Update()
    {
        // Detectar clic del mouse
        if (Input.GetMouseButton(0))
        {
            // Obtener la posición del mouse en el mundo
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                PaintTexture(hit.point); // Llama a la función de pintura
                Debug.Log("Painting");
            }
        }
    }
}
