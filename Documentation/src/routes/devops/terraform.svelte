<script>
    import PageTitle from "../../components/PageTitle.svelte";
    import Section from "../../components/Section.svelte";
</script>

<svelte:head>
    <title>Terraform</title>
</svelte:head>

<PageTitle title="Terraform">
    The system can be brought up to DigitalOcean in a managed kubernetes cluster.
</PageTitle>

<Section title="DOKS">
    The first resource created is the kubernetes cluster.
</Section>

<Section title="DOKS Config">
    The cluster is then configured by installing operators and required plugins such as a metrics server and rabbitmq operator.
    At last namespaces are also created.
</Section>

<Section title="Ingress Controller">
    The NGINX Ingress Controller is created with a DigitalOcean loadbalacer as an entrypoint to the system.
</Section>

<Section title="Certificate">
    Cert Manager is created and configured to manage certificates from lets-encrypt.
</Section>

<Section title="RabbitMQ">
    A RabbitMQ cluster is created as a Statefulset.
</Section>

<Section title="RavenDB">
    A single instance of RavenDB is created as a statefulset.
</Section>

<Section title="Logger">
    A single instance of Seq is created as a statefulset.
</Section>

<Section title="Minio">
    A single instance of Minio is created as a statefulset.
</Section>

<Section title="Microservices">
    All microservices are created as deployment, service, horizontal pod autoscaler, ingress rule and a configmap.
</Section>

<Section title="Records">
    Two A-records are created to point the loadbalancer to the domainname. It is seq.DOMAINNAME and DOMAINNAME.
</Section>