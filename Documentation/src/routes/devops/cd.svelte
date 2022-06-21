<script>
    import PageTitle from "../../components/PageTitle.svelte";
    import Section from "../../components/Section.svelte";
</script>

<svelte:head>
    <title>Continous Deployment</title>
</svelte:head>

<PageTitle title="Continous Deployment">
    All CD scripts are run on push to the master branch.
</PageTitle>

<Section title="Deploy Services">
    Each Service builds a Docker image, and then pushes it to its DockerHub repository.
</Section>

<Section title="Deploy NuGet">
    Each project being used as NuGet packages are deployed to the NuGet repository on GitHub.
    It is simply built and during building it is packed and at last it is pushed.
</Section>