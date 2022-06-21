import preprocess from 'svelte-preprocess';
import adapter from '@sveltejs/adapter-static';

/** @type {import('@sveltejs/kit').Config} */
const config = {
    // Consult https://github.com/sveltejs/svelte-preprocess
    // for more information about preprocessors
    preprocess: preprocess({
        postcss: true
    }),

    kit: {
        // hydrate the <div id="svelte"> element in src/app.html
        prerender: {
            default: true
        },
        adapter: adapter({
            pages: 'build',
            assets: 'build',
            fallback: null
        })
    }
};

export default config;