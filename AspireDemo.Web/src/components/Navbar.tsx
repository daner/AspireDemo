﻿import { Disclosure, DisclosureButton, DisclosurePanel } from '@headlessui/react'
import { Bars3Icon, XMarkIcon } from '@heroicons/react/24/outline'
import { NavigationItem } from "../models/Navigation";
import { NavLink } from 'react-router-dom'

interface IProps {
    navigation: NavigationItem[]
}

const classNames = (...classes: string[]) => {
    return classes.filter(Boolean).join(' ')
}

const Navbar = ({ navigation }: IProps) => {
    return (
        <>
            <Disclosure as="nav" className="bg-white shadow-sm">
                <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
                    <div className="flex h-16 justify-between">
                        <div className="flex">
                            <div className="flex flex-shrink-0 items-center">
                                <img
                                    alt="Your Company"
                                    src="https://tailwindui.com/img/logos/mark.svg?color=indigo&shade=600"
                                    className="block h-8 w-auto lg:hidden"
                                />
                                <img
                                    alt="Your Company"
                                    src="https://tailwindui.com/img/logos/mark.svg?color=indigo&shade=600"
                                    className="hidden h-8 w-auto lg:block"
                                />
                            </div>
                            <div className="hidden sm:-my-px sm:ml-6 sm:flex sm:space-x-8">
                                {navigation.map((item) => (
                                    <NavLink
                                        key={item.name}
                                        to={item.href}
                                        className={({ isActive }) => classNames(
                                            isActive
                                                ? 'border-indigo-500 text-gray-900'
                                                : 'border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700',
                                            'inline-flex items-center border-b-2 px-1 pt-1 text-sm font-medium',
                                        )}
                                    >
                                        {item.name}
                                    </NavLink>
                                ))}
                            </div>
                        </div>
                        <div className="-mr-2 flex items-center sm:hidden">
                            {/* Mobile menu button */}
                            <DisclosureButton
                                className="group relative inline-flex items-center justify-center rounded-md bg-white p-2 text-gray-400 hover:bg-gray-100 hover:text-gray-500 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2">
                                <span className="absolute -inset-0.5" />
                                <span className="sr-only">Open main menu</span>
                                <Bars3Icon aria-hidden="true" className="block h-6 w-6 group-data-[open]:hidden" />
                                <XMarkIcon aria-hidden="true" className="hidden h-6 w-6 group-data-[open]:block" />
                            </DisclosureButton>
                        </div>
                    </div>
                </div>

                <DisclosurePanel className="sm:hidden">
                    <div className="space-y-1 pb-3 pt-2">
                        {navigation.map((item) => (
                            <NavLink key={item.name}
                                to={item.href}
                                className={({ isActive }) => classNames(
                                    isActive
                                        ? 'border-indigo-500 bg-indigo-50 text-indigo-700'
                                        : 'border-transparent text-gray-600 hover:border-gray-300 hover:bg-gray-50 hover:text-gray-800',
                                    'block border-l-4 py-2 pl-3 pr-4 text-base font-medium',
                                )}>
                                <DisclosureButton>
                                    {item.name}
                                </DisclosureButton>
                            </NavLink>
                        ))}
                    </div>
                </DisclosurePanel>
            </Disclosure>
        </>
    )
}

export default Navbar
