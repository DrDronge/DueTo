<script lang="ts">
    /** ---------- DTO and helper types ---------- */
    export interface Day   { id: number; label: string }
    export interface TaskDto {
        id?: string;
        text?: string;
        color?: string;     // HEX/HSL etc.
        type?: string;
        priority?: 'low' | 'medium' | 'high';
        isDone?: boolean;
        activeDays?: Day[];
    }

    /**
     * ---------- Props ----------
     * `$props()` replaces `export let …` in Svelte 5.
     * You can destructure AND type the props in one go.
     */
    const { task, onUpdate = () => {} } = $props<{
        task: TaskDto;
        onUpdate?: (t: TaskDto) => void;
    }>();

    /* Make a local reactive copy so we can mutate freely */
    let local = $state({ ...task });

    function toggleDone() {
        local.isDone = !local.isDone;
        onUpdate({ ...local });
    }

    /** ---------- Pre‑computed colours ---------- */
    const accent   = local.color ?? '#3B82F6';      // fallback blue‑500
    const priority = local.priority ?? 'low';

</script>

<article
        class="card"
        style="--accent: {accent}; --priority: { priority };"
        aria-label={`Task ${local.text}`}
>
    <div class="accent"></div>         <!-- left coloured bar -->
    <div class="body">
        <header>
            <h2 class:is-done={local.isDone}>{local.text}</h2>
            <span class="badge">{local.priority}</span>
        </header>

        {#if local.type}
            <p class="type">{local.type}</p>
        {/if}

        {#if local.activeDays?.length}
            <ul class="days">
                {#each local.activeDays as d}
                    <li>{d.label}</li>
                {/each}
            </ul>
        {/if}

        <footer>
            <label class="checkbox">
                <input
                        type="checkbox"
                        checked={local.isDone}
                        onchange={toggleDone}
                />
                <span>{local.isDone ? 'Done' : 'Mark done'}</span>
            </label>
        </footer>
    </div>
</article>

<style>
    .card {
        --radius: 0.8rem;
        --gap: 1.2rem;
        --bg: #1e1e2f;
        display: grid;
        grid-template-columns: 0.6rem 1fr;
        background: var(--bg);
        border-radius: var(--radius);
        box-shadow: 0 0.4rem 0.8rem rgba(0,0,0,0.45);
        overflow: hidden;
        transition: transform 160ms, box-shadow 160ms;
    }
    .card:hover          { transform: translateY(-0.2rem); }
    .card:focus-within   { box-shadow: 0 0 0 0.2rem var(--accent); }
    .accent              { background: var(--accent); }

    .body   { padding: var(--gap); display: flex; flex-direction: column; gap: var(--gap); }
    header  { display: flex; align-items: center; justify-content: space-between; gap: 1rem; }
    h2      { margin: 0; font-size: 1.15rem; font-weight: 600; line-height: 1.4; }
    h2.is-done { text-decoration: line-through; opacity: 0.6; }

    .badge  {
        background: var(--priority);
        color: #000;
        border-radius: 0.4rem;
        padding: 0 0.6rem;
        font-size: 0.75rem;
        font-weight: 600;
        text-transform: uppercase;
        letter-spacing: 0.05em;
    }
    .type   { font-size: 0.8rem; opacity: 0.8; }

    .days   { display: flex; gap: 0.4rem; flex-wrap: wrap; list-style: none; margin: 0; padding: 0; }
    .days li{
        background: #2d2d43;
        border-radius: 0.4rem;
        padding: 0.2rem 0.6rem;
        font-size: 0.7rem;
    }

    /* Checkbox styling */
    .checkbox { display: inline-flex; align-items: center; gap: 0.6rem; cursor: pointer; }
    .checkbox input {
        appearance: none;
        width: 1rem; height: 1rem;
        border: 0.15rem solid #cbd5e1;
        border-radius: 0.2rem;
        display: grid; place-content: center;
        transition: background 120ms, border-color 120ms;
    }
    .checkbox input:checked {
        background: var(--accent); border-color: var(--accent);
    }
    .checkbox input:checked::after {
        content: '✓'; font-size: 0.8rem; color: #000;
    }
</style>