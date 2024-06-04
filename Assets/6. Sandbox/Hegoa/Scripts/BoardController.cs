using System.Collections; using System.Collections.Generic; using UnityEngine; using TMPro;

public class BoardController : MonoBehaviour
{
    [SerializeField] List<GameObject> _plucks = new(); 
    [SerializeField] List<GameObject> _hiddenConnectors = new();
    List<(GameObject connector, GameObject pluck)> _activeConnectors = new();

    [SerializeField] GameObject _connectorPrefab; 
    [SerializeField] LineRenderer _lineRenderer;

    [SerializeField] int _health = 3;
    [SerializeField] GameObject _currentCaller; 
    [SerializeField] GameObject _currentReceiver; 
    [SerializeField] bool _enableBoard = false;

    enum city {Madrid, Lebrija, Seville, Cadiz, Toledo}
    enum home {Duplex, Apartment, Chalet, Terraced}
    enum contract {Rented, Property}
    enum job {Mechanic, Professor, Firefighter, Baker, Police, Cashier}
    enum maritalStatus {Divorced, Married, Widowed, Single}
    enum socialClass {Low, Middle, High}
    
    private void Start()
    {
        SetLabels();
        NextCaller();
        CreateLabel();
    }

    public void OnSwitchClick(GameObject button)
    {
        if (!_enableBoard) return;

        if (_activeConnectors.Count < 2)
        {
            _hiddenConnectors[0].transform.position = button.transform.position;
            _hiddenConnectors[0].SetActive(true);
            _activeConnectors.Add(new(_hiddenConnectors[0], button.transform.parent.gameObject));
            _hiddenConnectors.RemoveAt(0);
        }
        if (_activeConnectors.Count == 2)
        {
            _lineRenderer.enabled = true;
            _lineRenderer.SetPosition(0, new(_activeConnectors[0].connector.transform.position.x, _activeConnectors[0].connector.transform.position.y, 0));
            _lineRenderer.SetPosition(1, new(_activeConnectors[1].connector.transform.position.x, _activeConnectors[1].connector.transform.position.y, 0));
        }
    }

    public void RemoveConnector(GameObject gameobject)
    {
        foreach ((GameObject connector, GameObject pluck) tuple in _activeConnectors)
        {
            if (tuple.connector == gameobject) 
            { 
                _activeConnectors.Remove(tuple);
                break;
            }
        }

        _hiddenConnectors.Add(gameobject);
        gameobject.SetActive(false);
        _lineRenderer.enabled = false;
    }

    public void NextCaller()
    {
        _currentCaller = _plucks[UnityEngine.Random.Range(0, _plucks.Count)];

        do {
            _currentReceiver = _plucks[UnityEngine.Random.Range(0, _plucks.Count)];
        } while (_currentReceiver == _currentCaller);

        _enableBoard = true;
    }

    public void ConfirmSelection()
    {
        if (_activeConnectors.Count < 2) return;

        RemoveConnector(_activeConnectors[0].connector);
        RemoveConnector(_activeConnectors[0].connector);

        int matches = 0;
        foreach ((GameObject connector, GameObject pluck) in _activeConnectors) if (pluck == _currentCaller || pluck == _currentReceiver) matches++;

        if (matches < 2) SetHealth(_health - 1);
        NextCaller();
    }

    private void SetHealth(int health)
    {
        this._health = health;
    }

    private void SetLabels()
    {
        foreach(GameObject pluck in _plucks)
        {
            string label;
            bool isValid;
            do
            {
                isValid = true;
                label = Random.Range(0, 999999).ToString("D6");
                foreach (GameObject p in _plucks)
                {
                    if (label == p.GetComponentInChildren<TextMeshProUGUI>().text) isValid = false;

                }
            } while (!isValid);

            pluck.GetComponentInChildren<TextMeshProUGUI>().text = label;
        }
    }

    private void CreateLabel1()
    {
        string label = "";
        int id1 = Random.Range(0, 5); int id2 = Random.Range(0, 4); int id3 = Random.Range(0, 2); 
        int id4 = Random.Range(0, 6); int id5 = Random.Range(0, 4); int id6 = Random.Range(0, 3);
        string labelId = "" + id1 + id2 + id3 + id4 + id5 + id6;

        
    }

    private void CreateLabel()
    {
        string label = Random.Range(0, 999999).ToString("D6");
        int id1 = 0; int id2 = 0; int id3 = 0; int id4 = 0; int id5 = 0; int id6 = 0;

        int digits12 = int.Parse(label.Substring(0, 2));
        if      ((digits12 >= 00 && digits12 <= 08) || (digits12 >= 55 && digits12 <= 64) || digits12 == 93)                     id1 = 0; // Madrid
        else if ((digits12 >= 31 && digits12 <= 40) || (digits12 >= 71 && digits12 <= 74) || (digits12 >= 94 && digits12 <= 99)) id1 = 1; // Lebrija
        else if ((digits12 >= 16 && digits12 <= 22) || (digits12 >= 41 && digits12 <= 46) || (digits12 >= 82 && digits12 <= 88)) id1 = 2; // Sevilla
        else if ((digits12 >= 09 && digits12 <= 15) || (digits12 >= 65 && digits12 <= 70) || (digits12 >= 75 && digits12 <= 81)) id1 = 3; // Cadiz
        else if ((digits12 >= 23 && digits12 <= 30) || (digits12 >= 47 && digits12 <= 54) || (digits12 >= 89 && digits12 <= 92)) id1 = 4; // Toledo


        int digit3 = int.Parse(label.Substring(2, 1)); int digit3Parity = digit3 / 2;
        int digit4 = int.Parse(label.Substring(3, 1)); int digit4Parity = digit4 / 2;
        if      (digit3Parity == 0 && digit4Parity != 0) id2 = 0; //Madrid-Sevilla  // Duplex         //Cadiz-Lebrija   //Adosado
        else if (digit3Parity != 0 && digit4Parity == 0) id2 = 1;                   // Apartamento                      // Chalet
        else if (digit3Parity == 0 && digit4Parity == 0) id2 = 2;                   // Chalet                           // Duplex
        else if (digit3Parity != 0 && digit4Parity != 0) id2 = 3;                   // Adosado                          //Apartamento
        if (id1 == 4 || id1 == 2) id2 += 4;


        int digit6 = int.Parse(label.Substring(5, 1)); bool isPrime = false;
        if (digit6 == 2 || digit6 == 3 || digit6 == 5 || digit6 == 7) isPrime = true;
        if (!isPrime) { if      (id1 == 1 || id1 == 4)               id3 = 0; // Alquiler
                        else if (id2 == 1 || id2 == 3)               id3 = 1; // Propiedad
                        else if (id2 != 1 && id2 != 3)               id3 = 0;
                        else if (id1 == 2 && id2 != 2)               id3 = 1;
                        else if (id2 == 2)                           id3 = 0;
                        else                                         id3 = 1;}

        else          { if      (id2 == 0 || id2 == 6)               id3 = 1;
                        else if (id2 != 3 && (id1 == 0 || id1 == 2)) id3 = 0;
                        else if (id1 == 1 || id1 == 3 || id1 == 4)   id3 = 1;
                        else                                         id3 = 0;}


        int digit5 = int.Parse(label.Substring(4, 1)); 
        int sub = digit3 - digit5; int sum = digit4 - digit6;
        if(sub < 0) { if      (sum <= 6)  id4 = 0;  // Mecanico
                      else if (sum <= 11) id4 = 1;  // Profesor
                      else if (sum <= 18) id4 = 2;} // Bombero

        else        { if      (sum <= 4)  id4 = 3;  // Panadero
                      else if (sum <= 9)  id4 = 4;  // Policia
                      else if (sum <= 18) id4 = 5;} // Cajero


        int digit1 = int.Parse(label.Substring(0, 1));

        string labelId = "" + id1 + id2 + id3 + id4 + id5 + id6;
    }
}
