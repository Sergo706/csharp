<script setup lang="ts">
const { open } = useContentSearch();
const searchTerm = ref('');

const { data: navigation } = await useAsyncData('navigation', () => {
    return queryCollectionNavigation('tools');
});

const { data: files, status } = useLazyAsyncData('search', () => {
    return queryCollectionSearchSections('tools');
}, {
    server: false
});

const groups = [{
    id: 'links',
    label: 'Explore',
    slot: 'links' as const,
    items: [
        { label: 'All Tools', icon: 'i-lucide-wrench', to: '/tools' },
        { label: 'PDF to Markdown', icon: 'i-lucide-file-text', to: '/tools/pdf-to-md' },
        { label: 'Blog', icon: 'i-lucide-newspaper', to: '/blog' },
        { label: 'Account', icon: 'i-lucide-user', to: '/account', external: true, target: '_self' },
        { label: 'About', icon: 'i-lucide-info', to: '/about' },
    ]
}];
</script>

<template>
  <UContentSearchButton
    :collapsed="true"
    variant="ghost"
    icon="i-lucide-search"
    @click="open = true"
  />

  <ClientOnly>
    <LazyUContentSearch
      v-model:search-term="searchTerm"
      :files="files"
      shortcut="meta_k"
      :navigation="navigation"
      :fuse="{ resultLimit: 42 }"
      :loading="status === 'pending'"
      :groups
      placeholder="Search tools..."
    />
  </ClientOnly>
</template>