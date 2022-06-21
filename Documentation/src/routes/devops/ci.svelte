<script>
    import PageTitle from "../../components/PageTitle.svelte";
    import Section from "../../components/Section.svelte";
</script>

<svelte:head>
    <title>Continous Integration</title>
</svelte:head>

<PageTitle title="Continous Integration">
    There is one CI script, which only handles dotnet projects, by restoring, building and testing.
</PageTitle>

<Section title="test">
    Runs all tests in .Tests projects, which only contain unit tests.
    All integration and load tests are run in console applications and are not part of the CI pipeline.
</Section>