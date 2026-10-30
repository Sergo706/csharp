<script setup lang="ts">

const { data: navLinks } = await useAsyncData<NavigationCollection>('navLinks', async () => {
  return await queryCollection('navigationMenu').first() as unknown as NavigationCollection;
});

provide('navLinks', navLinks);
</script>

<template>
  <UApp>
    <NuxtLoadingIndicator
      color="var(--ui-primary)"
      :height="2"
    />
    <LayoutHeader />
    <UMain class="lg:pt-[calc(var(--ui-header-height))]">
      <NuxtLayout>
        <NuxtRouteAnnouncer />
        <NuxtPage />
      </NuxtLayout>
    </UMain>
    <LayoutFooter />
  </UApp>
</template>