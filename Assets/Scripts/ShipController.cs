using UnityEngine;

public class ShipController : MonoBehaviour
{

    public float speed = 60f;
    public GameObject Home;

    private Planet _planet;

    public void SetPlanet(Planet planet)
    {
        _planet = planet;
        transform.position = PolarSystem.Position(_planet.startAngle, _planet.planetAtmosphereRadius, _planet.transform.position);
        speed = planet.shipSpeed;
        transform.rotation = Quaternion.Euler(0, 0, _planet.startAngle);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float angleZ = transform.rotation.eulerAngles.z;
            BlockInfo info = _planet.AddBlock(angleZ);

            GameObject childHome = Instantiate(Home, gameObject.transform.position, Quaternion.identity);

            HomeController homeController = childHome.GetComponent<HomeController>();
            homeController.homeInfo = info;
            homeController.shipPosition = transform.position;
            homeController.HomeBuild();

            childHome.transform.SetParent(_planet.transform);
            _planet.addBlockToBlocksList(childHome);
        }

        if (_planet.isClockwise)
            transform.RotateAround(_planet.transform.position, new Vector3(0.0f, 0.0f, -1.0f), Time.deltaTime * speed);
        else
            transform.RotateAround(_planet.transform.position, new Vector3(0.0f, 0.0f, 1.0f), Time.deltaTime * speed);
    }
}
