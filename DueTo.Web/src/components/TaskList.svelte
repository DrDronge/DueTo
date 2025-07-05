<svelte:options runes />

<script lang="ts">
    import TaskCard from '../components/TaskCard.svelte';
    import type { TaskDto } from '../components/TaskCard.svelte';


    /* Incoming list from parent or store */
    const { tasks = [] } = $props<{ tasks: TaskDto[] }>();

    /* Handle child callbacks */
    function updateTask(t: TaskDto) {
        const i = tasks.findIndex((x: TaskDto) => x.id === t.id);
        if (i !== -1) tasks[i] = t;
        // Persist to backend here …
    }
</script>

<section class="grid">
    {#if tasks.length === 0}
        <p>No tasks yet…</p>
    {:else}
        {#each tasks as task (task.id)}
            <TaskCard {task} onUpdate={updateTask} />
        {/each}
    {/if}
</section>

<style>
    .grid {
        display: grid;
        grid-template-columns: repeat(auto-fill, minmax(22rem, 1fr));
        gap: 1.6rem;
    }
</style>
