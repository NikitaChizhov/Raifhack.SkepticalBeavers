from diagrams import Diagram, Edge, Cluster
from diagrams.custom import Custom
from diagrams.onprem.database import PostgreSQL

with Diagram("\n\nCommand Query Responsibility Segregation toy project", show=False, direction="TB"):

    empty1 = Custom("", "transparent.png")
    empty2 = Custom("", "transparent.png")

    with Cluster("Application"):
        with Cluster("Queries controller"):
            queries = Custom("", "net5.png")

        with Cluster("Event handler (Hosted Service)"):
            hosted_service = Custom("", "net5.png")

        with Cluster("Commands controller"):
            commands = Custom("", "net5.png")

    event_store = Custom("Event Store", "eventstore.png")
    postgres = PostgreSQL("PostgreSQL")

    empty1 >> Edge(label="HTTP Get", minlen="1") >> queries
    queries >> postgres
    empty2 >> Edge(label="HTTP Post/Put/Delete", minlen="1") >> commands >> event_store
    event_store >> Edge(label="events") >> hosted_service
    hosted_service >> postgres


