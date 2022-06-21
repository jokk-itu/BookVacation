<script>
    import PageTitle from "../../components/PageTitle.svelte";
</script>

<svelte:head>
    <title>Mediator</title>
</svelte:head>

<PageTitle title="Mediator">
    Project to abstract away usage of the MediatR nuget package, by adjusting the mediator pipeline with logging and parameter validation using fluentvalidation.
    It also exposes the CQRS pattern by using Commands and Queries instead of the generic Request objects.
    Commands and Queries also return a Response object containing a generic type, a responsecode and a list of possible errors.
</PageTitle>