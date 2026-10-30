<script setup lang="ts">
import type { NavigationMenuItem } from '@nuxt/ui';
import type { NavigationCollection } from '~~/shared/types/Navigation';
import type { ContentNavigationItem } from '@nuxt/content';
const route = useRoute();



const navLinks = inject<Ref<NavigationCollection>>('navLinks');
const sidebarDocsNavigation = inject<Ref<ContentNavigationItem[]>>('sidebar_docs_navigation');

const desktopItems = computed<NavigationMenuItem[]>(() => {
  if (!navLinks?.value.links) return [];

  return navLinks.value.links.map((link): NavigationMenuItem => {
    return {
      label: link.label,
      icon: link.icon,
      to: link.to,
      active: link.nested ? route.path.startsWith(link.to) : route.path === link.to,
      children: link.nested ? link.children.map(c => ({
        label: c.label,
        description: c.description,
        icon: c.icon,
        to: c.to,
      })) : undefined
    };
  });
});


const mobileItems = computed<NavigationMenuItem[]>(() => {
  if (!navLinks?.value.links) return [];

  const items = navLinks.value.links.map((link): NavigationMenuItem => ({
    label: link.label,
    icon: link.icon,
    to: link.to,
    target: link.to === '/account' ? '_self' : undefined,
    external: link.to === '/account' ? true : undefined,
    active: link.nested ? route.path.startsWith(link.to) : route.path === link.to,
    children: link.nested ? link.children : []

  }));

  items.push({
    label: 'Account',
    icon: "i-lucide-user",
    to: '/account',
    target: '_self',
    external: true,
  });
  
  return items;
});

const navigation = computed(() => {
  const navArray = sidebarDocsNavigation?.value;
  if (!navArray || navArray.length === 0) return [];

  return navArray[0]?.children ?? navArray;
});

</script>

<template>
  <UHeader class="bg-cream-50/90 backdrop-blur dark:bg-riavzon-950/90">
    <template #title>
      <LayoutLogo 
        :logo="'/favicon.svg'"
        :size="'md'"
        :is-text="true" 
      />
    </template>

    <UNavigationMenu 
      :items="desktopItems"
    />

    <template #right>
      <LayoutSearch />

      <UButton
        variant="outline"
        to="/account"
        target="_self"
        external
        icon="i-lucide-user"
        aria-label="Account"
        label="Account"
        class="hidden lg:flex"
      />
    </template>
    
    <template #body>
      <UNavigationMenu 
        :items="mobileItems"  
        orientation="vertical" 
        class="-mx-2.5" 
      />
      <USeparator 
        type="dashed"
        decorative
        class="mt-5 mb-5"
      />
      <UContentNavigation 
        variant="link"
        highlight
        :navigation="navigation" 
        :default-open="true"
      />
    </template>
  </UHeader>
</template>