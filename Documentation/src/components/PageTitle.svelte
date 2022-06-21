<script lang='ts'>
    export let title : string = "LOREM IPSUM";
    
    const id = title.toLowerCase().replace(' ', '-');
</script>

<div class="py-4">
    <h2 class="text-3xl" id="{id}">
        <a class="text-blue-400 header-anchor" href="#{id}">#</a>{title}
    </h2>
    <hr class="border-blue-400"/>
</div>

<slot />