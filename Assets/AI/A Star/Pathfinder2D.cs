using System.Collections.Generic;
using System.Collections;
using UnityEngine;

using System.Linq;

public class Pathfinder2D : MonoBehaviour
{
    public int maxPathfinderIterations = 128;
    public int updateTime = 20;

    [Space]

    public float nodeReachDistance = 0.5f;
    public float agentRadius = 0.5f;

    public LayerMask obstacleLayerMask;
    public bool circleCast = true;

    [Space]

    public float stopDistance = 0;
    public bool canStop = true;

    [Header("Debug")]
    public bool stepByStepMode = false;
    public float stepTime = 0.1f;
    [Space]
    public bool showLineRenderer;
    public LineRenderer lineRenderer;
    List<Vector2> linePath = new List<Vector2>();

    //La lista de puntos del camino
    [HideInInspector]
    public List<Vector2> path = new List<Vector2>();

    List<BreadCrumb> openList = new List<BreadCrumb>(256);
    //En la closed list se guardan los breadCrumb que ya se procesaron.
    //Se agrega primero el del nodo de origen, ya que no necesita ser considerado.
    List<BreadCrumb> closedList = new List<BreadCrumb>();

    public Vector2 destination;
    public Vector2 GetDestination { get { return destination; } }
    public void SetDestination(Vector2 destination) { this.destination = destination; }

    Vector2 nextTarget;
    public Vector2 GetNextTarget { get { return nextTarget; } }
    public Vector2 GetMoveTarget { get { return inStopDistance ? Vector2.zero : nextTarget - (Vector2)transform.position; } }
    public virtual Vector2 CalculateNextTarget()
    {
        return path.FirstOrDefault();
    }

    bool inStopDistance = false;
    public bool InStopDistance { get { return inStopDistance; } }
    bool CalculateIfInStopDistance()
    {
        return IsCloseTo(destination, stopDistance) && canStop;
    }

    private void OnDrawGizmos()
    {
        if (!stepByStepMode) return;

        Gizmos.color = Color.yellow;
        foreach (var bc in openList)
        {
            Gizmos.DrawSphere(bc.node.GetPosition, 0.1f);
        }

        Gizmos.color = Color.red;
        foreach (var bc in closedList)
        {
            Gizmos.DrawSphere(bc.node.GetPosition, 0.1f);
            if (bc.previous != null)
            {
                Gizmos.DrawLine(bc.node.GetPosition, (bc.node.GetPosition + bc.previous.node.GetPosition) * .5f);
            }
        }
    }

    private void Start()
    {
        if (stepByStepMode)
        {
            StartCoroutine(GetPath(transform.position, destination, maxPathfinderIterations));
        }
    }

    private void Update()
    {
        if (stepByStepMode) return;

        //Cuando nos acercamos lo suficiente a un nodo, lo borramos.
        if (path.Count > 0 && IsCloseTo(path.FirstOrDefault(), nodeReachDistance))
        {
            path.RemoveAt(0);
            UpdateLineRenderer();
        }

        if (Time.frameCount % updateTime == 1)
        {
            // If we are not in the stopping distance, get a path
            if (!inStopDistance)
            {
                StartCoroutine(GetPath(transform.position, destination, maxPathfinderIterations));
            }
        }

        //Si el objetivo esta lejos, calcular el siguiente nodo objetivo
        if (!inStopDistance) nextTarget = CalculateNextTarget();

        //Calcular si nos tenemos que detener
        inStopDistance = CalculateIfInStopDistance();
    }

    void UpdateLineRenderer()
    {
        if (showLineRenderer && lineRenderer)
        {
            lineRenderer.positionCount = linePath.Count;
            lineRenderer.SetPositions(System.Array.ConvertAll(linePath.ToArray(), v2 => new Vector3(v2.x, v2.y)));
        }
        else if (lineRenderer && lineRenderer.enabled)
        {
            lineRenderer.enabled = showLineRenderer;
        }
    }

    protected bool IsCloseTo(Vector2 point, float distance)
    {
        if (distance == 0) return false;

        return Vector2.SqrMagnitude(point - (Vector2)transform.position) < distance * distance && CanGoToPoint(point);
    }

    protected bool CanGoToPoint(Vector2 point)
    {
        return CanGoToPoint(point, agentRadius);
    }
    protected bool CanGoToPoint(Vector2 point, float radius)
    {
        return circleCast
            ? !Physics2D.CircleCast(transform.position, radius, point - (Vector2)transform.position,
            Vector2.Distance(point, transform.position), obstacleLayerMask)
            : !Physics2D.Linecast(transform.position, point, obstacleLayerMask);
    }

    /// <summary>
    /// Método que devuelve un array de puntos a seguir, siempre devolverá el más corto que llegue al destino,
    /// y si no llega, el más cercano.
    /// </summary>
    /// <param name="origin">La posición de origen</param>
    /// <param name="destination">La posición de origen</param>
    /// <returns></returns>
    public IEnumerator GetPath(Vector2 origin, Vector2 destination)
    {
        return GetPath(origin, destination, int.MaxValue);
    }
    /// <summary>
    /// Método que devuelve un array de puntos a seguir, siempre devolverá el más corto que llegue al destino,
    /// y si no llega, el más cercano.
    /// </summary>
    /// <param name="origin">La posición de origen</param>
    /// <param name="destination">La posición de origen</param>
    /// <param name="maxIterations">La cantidad máxima que repite la búsqueda de nodos</param>
    /// <returns></returns>
    public IEnumerator GetPath(Vector2 origin, Vector2 destination, int maxIterations)
    {
        path.Clear();

        //Verificar que el NavMesh2D exista
        NavigationMesh2D navMesh = NavigationMesh2D.FindMap(transform.position);
        if (navMesh == null)
        {
            Debug.LogWarning("No 2D Navigation Mesh found.");
            enabled = false;
            yield break;
        }

        //Tomar los nodos válidos más cercanos al origen y destino
        Node originNode = navMesh.GetNearestValidNode(origin);
        Node destinationNode = navMesh.GetNearestValidNode(destination);

        //En la open list se guardan los breadCrumb que se van a considerar y ser procesados.
        openList.Clear();
        //En la closed list se guardan los breadCrumb que ya se procesaron.
        //Se agrega primero el del nodo de origen, ya que no necesita ser considerado.
        closedList.Clear();
        closedList.Add(new BreadCrumb(originNode));

        //Esta variable nos indica que breadCrumb estamos procesando.
        BreadCrumb currentBc = closedList[0];

        //Procesar el breadCrumb actual, repetir llegar al destino o se superen las iteraciones.
        int iterations = 0;
        while (iterations < maxIterations && currentBc != null && currentBc.node != destinationNode)
        {
            if (stepByStepMode) yield return new WaitForSeconds(stepTime);
            currentBc = FindBestBreadCrumb(currentBc, destinationNode, openList, closedList);
            iterations++;
        }

        //Transformar el BreadCrumb más cercano al objetivo en un camino a partir de sus
        //BreadCrumbs anteriores (previous)
        float lowestHeuristicCost = closedList.Min(bc => bc.heuristicCost);
        currentBc = closedList.Find(bc => bc.heuristicCost <= lowestHeuristicCost);
        Node closestNode = currentBc?.node;

        //Creamos una lista 'camino invertido' y agregamos las posiciones de los BreadCrumbs hasta que no
        //haya un anterior BreadCrumb.
        linePath.Clear();
        List<Vector2> reversedPath = new List<Vector2>();
        while (currentBc != null)
        {
            Vector3 position = currentBc.node.GetPosition;
            linePath.Add(position);

            if (stepByStepMode)
            {
                yield return new WaitForSeconds(stepTime);
                UpdateLineRenderer();
            }

            reversedPath.Add(position);
            currentBc = currentBc.previous;
        }

        
        List<Vector2> pathToFollow = reversedPath;
        if (closestNode == destinationNode)
        {
            pathToFollow = pathToFollow.Skip(1).Reverse().ToList();
            pathToFollow.Add(destination);
        }
        else pathToFollow.Reverse();

        //Devolver el camino salteando el primer nodo.
        path.Clear();
        path.AddRange(pathToFollow.Skip(1).ToArray());

        if (!stepByStepMode)
        {
            openList.Clear();
            closedList.Clear();

            linePath.Clear();
            linePath.AddRange(path);
            UpdateLineRenderer();
        }
    }

    /// <summary>
    /// Procesa un breadCrumb, primero inspeccionando los nodos adyacentes, agregandolos a la lista correspondiente.
    /// <para>Despues, de toda la openList se agrega a la closedList el breadCrumb con el costo más bajo,
    /// dando prioridad a los que fueron agregados últimos</para>
    /// </summary>
    /// <param name="breadCrumb">El BreadCrumb actual</param>
    /// <param name="destinationNode">El nodo al que queremos llegar</param>
    /// <param name="openList">La open list que estamos usando</param>
    /// <param name="closedList">La closed list que estamos usando</param>
    /// <returns></returns>
    static BreadCrumb FindBestBreadCrumb(BreadCrumb breadCrumb, Node destinationNode, List<BreadCrumb> openList, List<BreadCrumb> closedList)
    {
        //Primero inspeccionar cada nodo vecino
        foreach (NodePath path in breadCrumb.node.adyacentNodePaths)
        {
            //Si el camino es invalido o el breadcrumb está en la lista cerrada, saltear esta iteración
            if (!path.IsValid || closedList.Any(d => d.node == path.GetNode)) continue;

            //Encontrar si un BreadCrum ya existe
            BreadCrumb existingBreadCrumb = openList.Find(bc => bc.node == path.GetNode);
            if (existingBreadCrumb == null) //Sino existe, agregarlo a la lista
            {
                openList.Add(new BreadCrumb(path, breadCrumb, destinationNode));
            }
            else //En caso contrario, crear un breadcrumb nuevo y que reemplace al viejo si tiene un coste menor
            {
                BreadCrumb newBreadCrumb = new BreadCrumb(path, breadCrumb, existingBreadCrumb.heuristicCost);
                if (newBreadCrumb.GetTotalCost < existingBreadCrumb.GetTotalCost)
                {
                    existingBreadCrumb = newBreadCrumb;
                }
            }
        }

        // Después, encontrar el BreadCrumb con el menor costo
        float lowestCost = Mathf.Infinity;
        BreadCrumb lowestBreadCrumb = null;
        foreach (BreadCrumb openBc in openList.Reverse<BreadCrumb>())
        {
            float cost = openBc.GetTotalCost;
            if (cost < lowestCost)
            {
                lowestBreadCrumb = openBc;
                lowestCost = cost;
            }
        }

        //Si no hay, devolver nulo
        if (lowestBreadCrumb == null) return null;

        //Eliminar el breadCrumb de la openList y agregarlo a la closedList
        openList.Remove(lowestBreadCrumb);
        closedList.Add(lowestBreadCrumb);

        return lowestBreadCrumb;
    }

    /// <summary>
    /// Clase usada por el pathfinding que sirve como marca para un nodo, que incluye el
    /// breadCrumb anterior y el siguiente.
    /// <para>También tiene información de cuanto custa llegar hasta el breadCrumb y cuanto costo heuristic tiene</para>
    /// </summary>
    private class BreadCrumb
    {
        //El nodo al que pertenece
        public Node node;

        //Cuanto cuesta llegar hasta allá
        public float moveCost = 0;
        //El coste mínimo posible al destino ignorando los obstáculos
        public float heuristicCost = Mathf.Infinity;
        //Suma de los dos anteriores
        public float GetTotalCost { get { return moveCost + heuristicCost; } }

        //El breadCrumb anterior
        public BreadCrumb previous;
        //El breadCrumb siguiente
        public BreadCrumb next;

        /// <summary>
        /// Crea un breadCrumb vacío (Se usa para el de origen, ya que empieza en la closed list)
        /// </summary>
        /// <param name="node">El nodo al que corresponde</param>
        public BreadCrumb(Node node)
        {
            this.node = node;
        }
        /// <summary>
        /// Crea un breadCrumb que incluye el camino que se sigue y el nodo de destino para calcular
        /// el costo heuristic.
        /// </summary>
        /// <param name="nodePath">El camino que se sigue</param>
        /// <param name="previous">El breadCrumb anterior al que se crea</param>
        /// <param name="destination">El nodo de destino para el costo heuristic</param>
        public BreadCrumb(NodePath nodePath, BreadCrumb previous, Node destination) : this(nodePath.GetNode)
        {
            heuristicCost = node.GetHeuristicCost(destination);
            Initialize(nodePath, previous);
        }
        /// <summary>
        /// Crea un breadCrumb que incluye el camino que se sigue y el costo heuristic para no volver a calcularlo
        /// </summary>
        /// <param name="nodePath"></param>
        /// <param name="previous"></param>
        /// <param name="heuristicCost"></param>
        public BreadCrumb(NodePath nodePath, BreadCrumb previous, float heuristicCost) : this(nodePath.GetNode)
        {
            this.heuristicCost = heuristicCost;
            Initialize(nodePath, previous);
        }

        /// <summary>
        /// Se llama al instanciar el breadCrumb
        /// </summary>
        /// <param name="nodePath">El camino que se toma</param>
        /// <param name="previous">El breadCrumb anterior</param>
        void Initialize(NodePath nodePath, BreadCrumb previous)
        {
            this.previous = previous;
            previous.next = this;

            //Se calcula el coste de movimiento a partir del coste de movimiento del anterior + el coste del camino
            moveCost = previous.moveCost + nodePath.GetConnection.GetCost;
        }
    }
}
